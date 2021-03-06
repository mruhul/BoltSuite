﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApi.Features.Books
{
    [ApiController]
    [Route("v1/books")]
    public class BooksController : ControllerBase
    {
        private readonly BooksStore store;

        public BooksController(BooksStore store)
        {
            this.store = store;
        } 

        [HttpPost]
        [Route("")]
        public IActionResult Post([FromBody]Book book)
        {
            if(string.IsNullOrWhiteSpace(book.Title))
            {
                return BadRequest(new { 
                    Errors = new[] 
                    {
                        new {
                            Code = "TitleRequired", 
                            Message = "Title is required.",
                            PropertyName = "Title"
                        }
                    }
                });
            }

            var id = store.Create(book);
            
            book.Id = id;

            return Created($"/v1/books/{book.Id}", book);
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(string id)
        {
            var book = store.GetById(id);

            if (book == null) return NotFound($"Book not found with id {id}");

            return Ok(book);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Put([FromRoute]string id, Book updatedBook)
        {
            var book = store.GetById(id);

            if (book == null) return NotFound($"Book not found with id {id}");

            updatedBook.Id = book.Id;

            store.Update(updatedBook);

            return Ok(updatedBook);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            var isDeleted = store.Delete(id);

            if (isDeleted)
            {
                store.Restore();

                return Ok();
            }

            return NotFound($"Book not found with id {id} to delete");
        }
    }
}
