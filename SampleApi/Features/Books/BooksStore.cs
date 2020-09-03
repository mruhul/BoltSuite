using Bolt.IocScanner.Attributes;
using System;
using System.Collections.Generic;

namespace SampleApi.Features.Books
{
    [AutoBind(LifeCycle.Singleton)]
    public class BooksStore
    {
        private Dictionary<string, Book> _books = new Dictionary<string, Book> { };

        public BooksStore()
        {
            Restore();
        }

        public void Restore()
        {
            _books = new Dictionary<string, Book>
            {
                ["1"] = new Book
                {
                    Id = "1",
                    Title = "book1"
                },
                ["3"] = new Book
                {
                    Id = "3",
                    Title = "book3"
                },
                ["4"] = new Book
                {
                    Id = "4",
                    Title = "book4"
                }
            };
        }

        public IEnumerable<Book> GetAll() => _books.Values;
        public Book GetById(string id) => _books.TryGetValue(id, out var book) ? book : null;
        public string Create(Book book) {
            var id = Guid.NewGuid().ToString();
            book.Id = id;
            _books[id] = book;
            return id;
        }
        public bool Update(Book book)
        {
            if(_books.ContainsKey(book.Id))
            {
                _books[book.Id] = book;
                return true;
            }
            return false;
        }
        public bool Delete(string id)
        {
            if(_books.ContainsKey(id))
            {
                return _books.Remove(id);
            }

            return false;
        }
    }
}
