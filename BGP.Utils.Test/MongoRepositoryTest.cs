using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Configuration;
using BGP.Utils.Repository;
using DryIoc;
using BGP.Utils.Test.Entities;
using MongoDB.Bson;
using System.Linq;
using System.IO;
using System.Reflection;
using BGP.Utils.Repository.Mongo;

namespace BGP.Utils.Test
{
    [TestClass]
    public class MongoRepositoryTest
    {
        private IRepository _repo;
        private Random _random;

        [TestInitialize]
        public void Setup()
        {
            // Create database
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            
            // Register services
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

            container.RegisterDelegate<IMongoDatabase>(x => new MongoClient(url).GetDatabase(url.DatabaseName));
            container.Register<IRepository, Repository.Mongo.Repository>();

            _repo = container.Resolve<IRepository>();
            _random = new Random();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            // Drop database
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString);
            var client = new MongoClient(url);
            client.DropDatabase(url.DatabaseName);
        }


        [TestMethod]
        public void BasicCRUDTest()
        {
            var category = new Category();
            category.CategoryID = _random.Next();
            category.Name = "Category 1";
            category.Description = "Description";

            _repo.Create(category);

            Assert.IsNotNull(category.Id);

            // fetch it back 
            var newlyAddedCategory = _repo.Find<Category>(c => c.CategoryID == category.CategoryID);

            Assert.IsNotNull(newlyAddedCategory);
            Assert.AreEqual(category.Id, newlyAddedCategory.Id);
            Assert.AreEqual(category.Name, newlyAddedCategory.Name);

            newlyAddedCategory.Name = "ABC XYZ";
            newlyAddedCategory.Description = "sdfsdfsdf";

            _repo.Update(newlyAddedCategory.Id, newlyAddedCategory);

            // fetch by id now 
            var updatedCategory = _repo.FindById<Category>(category.Id);

            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(newlyAddedCategory.Name, updatedCategory.Name);
            Assert.AreEqual(newlyAddedCategory.Description, updatedCategory.Description);

            // delete it
            _repo.DeleteById<Category>(category.Id);

            var deletedCategory = _repo.FindById<Category>(category.Id);

            Assert.IsNull(deletedCategory);
        }

        [TestMethod]
        public void ComplexCRUDTest()
        {
            var post = new Post();
            post.PostID = _random.Next();
            post.Subject = "My first post";
            post.Content = "Blah blah blah";
            post.Category = new Category()
            {
                CategoryID = _random.Next(),
                Name = "A new type"
            };
            post.PostedByUser = new User()
            {
                UserID = _random.Next(),
                FirstName = "Random",
                LastName = "Annonymous"
            };
            _repo.Create(post);

            Assert.IsNotNull(post.Id);

            // fetch it back 
            var newlyAddedPost = _repo.Find<Post>(c => c.Subject == post.Subject);

            Assert.IsNotNull(newlyAddedPost);
            Assert.AreEqual(post.Id, newlyAddedPost.Id);
            Assert.AreEqual(post.Subject, newlyAddedPost.Subject);
            Assert.AreEqual(post.Category.CategoryID, newlyAddedPost.Category.CategoryID);
            Assert.AreEqual(post.PostedByUser.FirstName, newlyAddedPost.PostedByUser.FirstName);

            newlyAddedPost.Content = "New content blah blah";
            newlyAddedPost.Category = new Category
            {
                CategoryID = _random.Next(),
                Name = "Other type"
            };

            _repo.Update(newlyAddedPost.Id, newlyAddedPost);

            // fetch by id now 
            var updatedPost = _repo.FindById<Post>(post.Id);

            Assert.IsNotNull(updatedPost);
            Assert.AreEqual(newlyAddedPost.Subject, updatedPost.Subject);
            Assert.AreEqual(newlyAddedPost.Category.CategoryID, updatedPost.Category.CategoryID);
            Assert.AreEqual(newlyAddedPost.Category.Name, updatedPost.Category.Name);

            // delete it
            _repo.DeleteById<Post>(post.Id);

            var deletedPost = _repo.FindById<Post>(post.Id);

            Assert.IsNull(deletedPost);
        }


    }
}
