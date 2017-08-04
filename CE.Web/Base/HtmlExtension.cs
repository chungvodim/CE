using CE.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using BGP.Utils.Common;
using CE.Dto;
using CE.Entity.Main;
using System.Text;
using System.Web.Routing;
using System.Linq.Expressions;
using System.Web.Mvc.Html;

namespace CE.Web
{
    public static class HtmlHelperExtension
    {
        public static IHtmlString ValidationSummary(this HtmlHelper html, BaseDto viewModel, bool? isAutoHidden = false)
        {
            if(viewModel != null && !string.IsNullOrWhiteSpace(viewModel.Message))
            {
                var p = new TagBuilder("p");
                p.SetInnerText(viewModel.Message);
                if (viewModel.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    p.AddCssClass("alert alert-success");
                }
                else
                {
                    p.AddCssClass("alert alert-error");
                }
                if(isAutoHidden != null && isAutoHidden == true)
                {
                    p.AddCssClass("alert-auto-hidden");
                }
                return html.Raw(HttpUtility.HtmlDecode(p.ToString(TagRenderMode.Normal)));
            }
            else
            {
                return null;
            }
        }

        public static IHtmlString DisplayResource(this HtmlHelper html, string rs)
        {
            return html.Raw(rs);
        }

        public static IHtmlString DisplayResourceJs(this HtmlHelper html, string rs)
        {
            return html.Raw(HttpUtility.JavaScriptStringEncode(rs));
        }

        public static IHtmlString DisplayResource(this HtmlHelper html, MvcHtmlString rs)
        {
            return html.Raw(HttpUtility.HtmlDecode(rs.ToString()));
        }

        public static string EnumParseToDescription<TEnum>(this HtmlHelper html, string input) where TEnum : struct
        {
            TEnum result;
            var isSuccess = System.Enum.TryParse(input, out result);
            if (isSuccess)
            {
                return result.GetDescription();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Creates a checkbox list for flag enums.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The model property type.</typeparam>
        /// <param name="html">The html helper.</param>
        /// <param name="expression">The model expression.</param>
        /// <param name="htmlAttributes">Optional html attributes.</param>
        /// <param name="sortAlphabetically">Indicates if the checkboxes should be sorted alfabetically.</param>
        /// <returns></returns>
        public static MvcHtmlString CheckBoxListForEnum<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null, bool sortAlphabetically = true)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullBindingName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var fieldId = TagBuilder.CreateSanitizedId(fullBindingName);

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var value = metadata.Model;

            // Get all enum values
            IEnumerable<TValue> values = System.Enum.GetValues(typeof(TValue)).Cast<TValue>();

            // Sort them alphabetically by resource name or default enum name
            if (sortAlphabetically)
                values = values.OrderBy(i => i.ToString());

            // Create checkbox list
            var sb = new StringBuilder();
            foreach (var item in values)
            {
                TagBuilder builder = new TagBuilder("input");
                long targetValue = Convert.ToInt64(item);
                long flagValue = Convert.ToInt64(value);

                if ((targetValue & flagValue) == targetValue)
                    builder.MergeAttribute("checked", "checked");

                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", item.ToString());
                builder.MergeAttribute("name", fieldId);

                // Add optional html attributes
                if (htmlAttributes != null)
                    builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                builder.InnerHtml = item.ToString();

                sb.Append(builder.ToString(TagRenderMode.Normal));

                // Seperate checkboxes by new line
                sb.Append("<br/>");
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString DropdownListForEnum<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string optionLabel = null, object htmlAttributes = null)
        {
            var selectList = System.Enum.GetValues(typeof(TValue)).Cast<TValue>().Where(x => (x as System.Enum).GetAttribute<IgnoredEnumAttribute>() == null).Select(x => new SelectListItem()
            {
                Value = (Convert.ToInt64(x)).ToString(),
                Text = (x as System.Enum).GetEnumDisplayText()
            });

            return html.DropDownListFor<TModel, TValue>(expression, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropdownListForEnumNullable<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, Nullable<TValue>>> expression, string optionLabel = null, object htmlAttributes = null) where TValue : struct, IComparable
        {
            var selectList = System.Enum.GetValues(typeof(TValue)).Cast<TValue>().Select(x => new SelectListItem()
            {
                Value = (Convert.ToInt64(x)).ToString(),
                Text = (x as System.Enum).GetEnumDisplayText()
            });

            return html.DropDownListFor<TModel, TValue?>(expression, selectList, optionLabel, htmlAttributes);
        }

        public static IHtmlString Label<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return Label(html, expression, new RouteValueDictionary());
        }
        public static IHtmlString Label<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return Label(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static IHtmlString Label<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            TagBuilder span = new TagBuilder("span");
            span.InnerHtml += labelText;
            if (expression.IsRequired())
            {
                TagBuilder i = new TagBuilder("i");
                i.AddCssClass("fa fa-asterisk red");
                i.MergeAttribute("aria-hidden", "true");
                span.InnerHtml += " " + i.ToString(TagRenderMode.Normal);
                //labelText += " <i class='fa fa-asterisk' aria-hidden='true'></i>";
            }

            // assign <span> to <label> inner html
            tag.InnerHtml = span.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}