using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace BGP.Utils.Common
{
    public class PdfHelper
    {
        // Some pre-defined colors
        // RGB colors
        private static readonly Color TableBorder = new Color(204, 204, 204);
        private static readonly Color TableBlue = new Color(246, 246, 246);

        // Document parts
        private PageFormat _pageSize = PageFormat.A4;
        private Orientation _pageOrientation = Orientation.Portrait;
        private Document _document;
        private Table _table;

        // User data
        private DataTable _data;
        private string _title;
        private string _description;
        private double[] _colWidths;
        private string[] _colNames;
        private List<PdfColumnStyle> _colStyles;
        private bool _useCustomStyle;

        public PageFormat PageFormat
        {
            set { _pageSize = value; }
        }

        public Orientation PageOrientation
        {
            set { _pageOrientation = value; }
        }

        /// <summary>
        /// Set font size for cell in table
        /// </summary>
        public int CellFontSize { get; set; }


        // default constructor
        public PdfHelper()
        {
        }

        // constructor with PageOrientation
        public PdfHelper(Orientation pageOrientation)
        {
            _pageOrientation = pageOrientation;
        }

        /// <summary>
        /// Generate PDF document
        /// </summary>
        /// <param name="title">Title of exported PDF</param>
        /// <param name="description">Small description below title</param>
        /// <param name="data">DataTable of export data</param>
        /// <param name="colWidths">Column width definitions (in percent)</param>
        /// <returns></returns>
        public byte[] GeneratePDF(string title, string description, DataTable data, double[] colWidths)
        {
            if (data.Columns.Count != colWidths.Length)
            {
                throw new Exception("Column width definitions doesn't match with DataTable.");
            }

            var columns = ConvertToCentimeter(colWidths);

            return GeneratePDFCentimeterUnit(title, description, data, columns);
        }

        public byte[] GeneratePDF(string title, string description, DataTable data, double[] colWidths, string[] colNames)
        {
            if (data.Columns.Count != colWidths.Length)
            {
                throw new Exception("Column width definitions doesn't match with DataTable.");
            }

            var columns = ConvertToCentimeter(colWidths);
            _colNames = colNames;

            return GeneratePDFCentimeterUnit(title, description, data, columns);
        }

        /// <summary>
        /// Generate PDF document
        /// </summary>
        /// <param name="title">Title of exported PDF</param>
        /// <param name="description">Small description below title</param>
        /// <param name="data">DataTable of export data</param>
        /// <param name="colStyles">Column style definitions</param>
        /// <returns></returns>
        public byte[] GeneratePDF(string title, string description, DataTable data, List<PdfColumnStyle> colStyles)
        {
            if (data.Columns.Count != colStyles.Count)
            {
                throw new Exception("Column width definitions doesn't match with DataTable.");
            }

            _useCustomStyle = true;
            _colStyles = colStyles;
            var colWidths = colStyles.Select(cs => cs.Width).ToArray();
            var columns = ConvertToCentimeter(colWidths);

            return GeneratePDFCentimeterUnit(title, description, data, columns);
        }

        /// <summary>
        /// Generate PDF document
        /// </summary>
        /// <param name="title">Title of exported PDF</param>
        /// <param name="description">Small description below title</param>
        /// <param name="data">DataTable of export data</param>
        /// <param name="colWidths">Column width definitions (in centimeter)</param>
        /// <returns></returns>
        public byte[] GeneratePDFCentimeterUnit(string title, string description, DataTable data, double[] colWidths)
        {
            if (data.Columns.Count != colWidths.Length)
            {
                throw new Exception("Column width definitions doesn't match with DataTable.");
            }

            _data = data;
            _title = title;
            _description = description;
            _colWidths = colWidths;
            if (_colNames == null)
            {
                _colNames = new string[] { };
            }

            // Create a new MigraDoc document
            _document = new Document();
            _document.Info.Title = title;
            _document.Info.Author = "Denovu";
            _document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(1);
            _document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(1);

            DefineStyles();

            CreatePage();

            FillContent();

            // Create a renderer for the MigraDoc document.
            var pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = _document;

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            byte[] fileContents;
            using (var stream = new MemoryStream())
            {
                pdfRenderer.Save(stream, true);
                fileContents = stream.ToArray();
            }

            return fileContents;
        }

        private double[] ConvertToCentimeter(double[] colWidths)
        {
            // Get the page size
            Unit width, height;
            PageSetup.GetPageSize(_pageSize, out width, out height);

            double pageWidth = _pageOrientation == Orientation.Portrait ? width.Centimeter : height.Centimeter;
            // page margin
            pageWidth = pageWidth - 2;

            var columns = new double[colWidths.Length];

            for (int i = 0; i < colWidths.Length; i++)
            {
                columns[i] = colWidths[i] * pageWidth;
            }

            return columns;
        }

        /// <summary>
        /// Defines the styles used to format the MigraDoc document.
        /// </summary>
        private void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = _document.Styles["Normal"];
            style.Font.Name = "Verdana";

            // Create a new style called Table based on style Normal
            style = _document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Helvetica";
            style.Font.Size = this.CellFontSize > 0 ? this.CellFontSize : 8;

            // Create a new style called Title based on style Normal
            style = _document.Styles.AddStyle("Title", "Normal");
            style.Font.Size = 18;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            // Create a new style called Description based on style Normal
            style = _document.Styles.AddStyle("Description", "Normal");
            style.Font.Size = 10;
            style.Font.Italic = true;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.ParagraphFormat.SpaceAfter = "5mm";
        }

        /// <summary>
        /// Creates the static parts of the invoice.
        /// </summary>
        private void CreatePage()
        {
            // Get the page size
            Unit width, height;
            PageSetup.GetPageSize(_pageSize, out width, out height);

            // Each MigraDoc document needs at least one section.
            Section section = _document.AddSection();

            // Set the page size
            section.PageSetup.Orientation = _pageOrientation;
            section.PageSetup.PageHeight = height;
            section.PageSetup.PageWidth = width;
            section.PageSetup.LeftMargin = 0;
            section.PageSetup.RightMargin = 0;

            // Add the Title
            var paragraph = section.AddParagraph();
            paragraph.Style = "Title";
            paragraph.AddFormattedText(_title.ToUpper(), TextFormat.Bold);

            paragraph = section.AddParagraph();
            paragraph.Style = "Description";
            paragraph.AddFormattedText(_description, TextFormat.Italic);

            // Create the item table
            _table = section.AddTable();
            _table.Style = "Table";
            _table.TopPadding = Unit.FromMillimeter(1.5);
            _table.BottomPadding = Unit.FromMillimeter(1.5);
            _table.Borders.Color = TableBorder;
            _table.Borders.Width = 0.25;
            _table.Borders.Left.Width = 0.5;
            _table.Borders.Right.Width = 0.5;

            double totalWidth = 0;
            // Before you can add a row, you must define the columns
            Column column;
            foreach (double colWidth in _colWidths)
            {
                totalWidth += colWidth;
                column = _table.AddColumn(Unit.FromCentimeter(colWidth));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            // center table on page
            _table.Rows.LeftIndent = Unit.FromCentimeter(((_pageOrientation == Orientation.Portrait ? width.Centimeter : height.Centimeter) - totalWidth) / 2);

            // Create the header of the table
            Row row = _table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;

            for (int i = 0; i < _data.Columns.Count; i++)
            {
                var colName = (_useCustomStyle && !string.IsNullOrEmpty(_colStyles[i].Name))
                                ? _colStyles[i].Name
                                : _colNames.Length > 0 ? _colNames[i] : _data.Columns[i].ColumnName;

                row.Cells[i].AddParagraph(colName);
                row.Cells[i].Format.Font.Bold = true;
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }

            _table.SetEdge(0, 0, _data.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        /// <summary>
        /// Creates the dynamic parts of the invoice.
        /// </summary>
        private void FillContent()
        {
            Row row1;
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                row1 = _table.AddRow();

                for (int j = 0; j < _data.Columns.Count; j++)
                {
                    row1.Cells[j].AddParagraph(_data.Rows[i][j].ToString());
                    row1.Cells[j].Format.Alignment = _useCustomStyle ? _colStyles[j].Alignment : ParagraphAlignment.Left;
                    row1.Cells[j].VerticalAlignment = _useCustomStyle ? _colStyles[j].VerticalAlignment : VerticalAlignment.Center;

                    _table.SetEdge(0, _table.Rows.Count - 1, _data.Columns.Count, 1, Edge.Box, BorderStyle.Single, 0.75);
                }
            }
        }
    }

    public class PdfColumnStyle
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public ParagraphAlignment Alignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        public PdfColumnStyle()
        {
            // Set default values
            Name = "";
            Alignment = ParagraphAlignment.Left;
            VerticalAlignment = VerticalAlignment.Center;
        }

        public PdfColumnStyle(double columnWidth)
        {
            Width = columnWidth;
        }

        public PdfColumnStyle(string name, double columnWidth, ParagraphAlignment horizontalAligment)
        {
            Name = name;
            Width = columnWidth;
            Alignment = horizontalAligment;
        }
    }
}