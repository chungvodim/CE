using BGP.Utils.Repository;
using BGP.Utils.Service;
using BGP.Utils.Test.Dtos;
using BGP.Utils.Test.Entities;
using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;

namespace BGP.Utils.Test
{
    [TestClass]
    public class MongoServiceTest
    {
        private IBaseService _repo;
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
            container.Register<Repository.Mongo.Repository>();
            container.Register<IBaseService, Service.Mongo.BaseService>();

            // Create mapping dtos <==> entities
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CategoryDto, Category>();
                cfg.CreateMap<Category, CategoryDto>();

                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<User, UserDto>();

                cfg.CreateMap<PostDto, Post>();
                cfg.CreateMap<Post, PostDto>();

                cfg.CreateMap<CommentDto, Comment>();
                cfg.CreateMap<Comment, CommentDto>();
            });

            _repo = container.Resolve<IBaseService>();
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
            var category = new CategoryDto();
            category.CategoryID = _random.Next();
            category.Name = "Category 1";
            category.Description = "Description";

            _repo.Create<Category, CategoryDto>(category);

            Assert.IsNotNull(category.Id);

            // fetch it back 
            var newlyAddedCategory = _repo.Find<Category, CategoryDto>(c => c.CategoryID == category.CategoryID);

            Assert.IsNotNull(newlyAddedCategory);
            Assert.AreEqual(category.Id, newlyAddedCategory.Id);
            Assert.AreEqual(category.Name, newlyAddedCategory.Name);

            newlyAddedCategory.Name = "ABC XYZ";
            newlyAddedCategory.Description = "sdfsdfsdf";

            _repo.Update<Category, CategoryDto>(newlyAddedCategory.Id, newlyAddedCategory);

            // fetch by id now 
            var updatedCategory = _repo.FindById<Category, CategoryDto>(category.Id);

            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(newlyAddedCategory.Name, updatedCategory.Name);
            Assert.AreEqual(newlyAddedCategory.Description, updatedCategory.Description);

            // delete it
            _repo.DeleteById<Category>(category.Id);

            var deletedCategory = _repo.FindById<Category, CategoryDto>(category.Id);

            Assert.IsNull(deletedCategory);
        }

        [TestMethod]
        public void ComplexCRUDTest()
        {
            var post = new PostDto();
            post.PostID = _random.Next();
            post.Subject = "My first post";
            post.Content = "Blah blah blah";
            post.Category = new CategoryDto()
            {
                CategoryID = _random.Next(),
                Name = "A new type"
            };
            post.PostedByUser = new UserDto()
            {
                UserID = _random.Next(),
                FirstName = "Random",
                LastName = "Annonymous"
            };
            _repo.Create<Post, PostDto>(post);

            Assert.IsNotNull(post.Id);

            // fetch it back 
            var newlyAddedPost = _repo.Find<Post, PostDto>(c => c.Subject == post.Subject);

            Assert.IsNotNull(newlyAddedPost);
            Assert.AreEqual(post.Id, newlyAddedPost.Id);
            Assert.AreEqual(post.Subject, newlyAddedPost.Subject);
            Assert.AreEqual(post.Category.CategoryID, newlyAddedPost.Category.CategoryID);
            Assert.AreEqual(post.PostedByUser.FirstName, newlyAddedPost.PostedByUser.FirstName);

            newlyAddedPost.Content = "New content blah blah";
            newlyAddedPost.Category = new CategoryDto
            {
                CategoryID = _random.Next(),
                Name = "Other type"
            };

            _repo.Update<Post, PostDto>(newlyAddedPost.Id, newlyAddedPost);

            // fetch by id now 
            var updatedPost = _repo.FindById<Post, PostDto>(post.Id);

            Assert.IsNotNull(updatedPost);
            Assert.AreEqual(newlyAddedPost.Subject, updatedPost.Subject);
            Assert.AreEqual(newlyAddedPost.Category.CategoryID, updatedPost.Category.CategoryID);
            Assert.AreEqual(newlyAddedPost.Category.Name, updatedPost.Category.Name);

            // delete it
            _repo.DeleteById<Post>(post.Id);

            var deletedPost = _repo.FindById<Post, PostDto>(post.Id);

            Assert.IsNull(deletedPost);
        }
    }
}
