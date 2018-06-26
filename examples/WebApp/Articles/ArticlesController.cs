using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Articles
{
    [Route("[controller]")]
    public class ArticlesController : Controller
    {
        private readonly AppContext _dbContext;

        public ArticlesController(AppContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet, ProducesResponseType(200, Type = typeof(List<Article>))]
        public async Task<IActionResult> GetAsync()
            => Ok(await _dbContext.Articles.ToListAsync());

        [HttpGet("{id}"), ProducesResponseType(200, Type = typeof(Article))]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _dbContext.Articles.FindAsync(id));

        [HttpPost, ProducesResponseType(201, Type = typeof(Article))]
        public async Task<IActionResult> PostAsync([FromBody] Article article)
        {
            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }
    }
}