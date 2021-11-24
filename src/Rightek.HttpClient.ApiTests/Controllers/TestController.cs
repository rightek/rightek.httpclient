using Microsoft.AspNetCore.Mvc;

namespace Rightek.HttpClient.ApiTests.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok();

        [HttpGet("{data}")]
        public IActionResult Get(int data) => Ok(data);

        [HttpPost]
        public IActionResult Post() => Ok();

        [HttpPost]
        public IActionResult Post(Post m) => Ok(m.Data);

        [HttpPost]
        public IActionResult PostXml(string xml) => Ok(xml);

        [HttpPost]
        public IActionResult PostForm([FromForm] Post m) => Ok(m.Data);

        // ToDo: add proper apis for other request types
    }

    public class Post
    {
        public int Data { get; set; }
    }
}