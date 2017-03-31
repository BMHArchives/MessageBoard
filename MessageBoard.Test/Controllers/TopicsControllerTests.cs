using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessageBoard.Controllers;
using MessageBoard.Test.Fakes;
using MessageBoard.Data;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Newtonsoft.Json;

namespace MessageBoard.Test.Controllers
{
    [TestClass]
    public class TopicsControllerTests
    {
        private TopicsController _ctrl;

        [TestInitialize]
        public void Init()
        {
            _ctrl = new TopicsController(new FakeMessageBoardRepository());
        }
        [TestMethod]
        public void TopicsController_Get()
        {
            //Writing some assumptions

            var results = _ctrl.Get(true);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() > 0);
            Assert.IsNotNull(results.First());
            Assert.IsNotNull(results.First().Title);
        }
        [TestMethod]
        public void TopicsController_Post()
        {
            // Required for testing a post request when testing.
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/v1/topics");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "topics" } });

            //injects the in the controller context.
            _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
            _ctrl.Request = request;
            _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            var newTopic = new Topic()
            {
                Title = "Test Topic",
                Body = "This is a test of a topic...."
            };

            var result = _ctrl.Post(newTopic);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var json = result.Content.ReadAsStringAsync().Result;
            var topic = JsonConvert.DeserializeObject<Topic>(json);
            Assert.IsNotNull(topic);
            Assert.IsTrue(topic.Id > 0);
            Assert.IsTrue(topic.Created > DateTime.MinValue);
        }
    }
}
