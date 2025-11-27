using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Chirp.Core.Repositories;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Models;

namespace Chirp.Razor.Repositories
{
    public class CheepRepository : ICheepRepository
    {
        private readonly ChirpDBContext _context;

        public CheepRepository(ChirpDBContext context)
        {
            _context = context;
        }

        public IEnumerable<CheepDTO> GetAll(int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .Select(createCheepDTO)
                .ToList();
        }

        public IEnumerable<CheepDTO> GetByAuthor(string authorName, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            authorName = authorName.Trim().ToLower();
            
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.Author.Name.ToLower() == authorName)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .Select(c => new CheepDTO
                {
                    Author = c.Author.Name,
                    Message = c.Text,
                    CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
                })
                .ToList();
        }

        public void CreateNewAuthor(string name, string email)
        {
            var author = new Author
            {
                Name = name,
                Email = email,
                Cheeps = new List<Cheep>()
            };

            _context.Authors.Add(author);
            _context.SaveChanges();
            
        }

        public AuthorDTO? FindAuthorByName(string name)
        {
            return _context.Authors
                .Where(a => a.Name.ToLower() == name.ToLower())
                .Select(a => new AuthorDTO
                {
                    Name = a.Name,
                    Email = a.Email,
                })
                .FirstOrDefault();
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            return _context.Authors
                .Where(a => a.Email.ToLower() == email.ToLower())
                .Select(a => new AuthorDTO
                {
                    Name = a.Name,
                    Email = a.Email,
                })
                .FirstOrDefault();
        }

        public void AddChirp(CheepDTO chirp)
            { 
            var author = _context.Authors.FirstOrDefault(a => a.Name == chirp.Author);
            if (author == null)
            {
                throw new InvalidOperationException($"No author found with name '{chirp.Author}'.");
            }

            if (!DateTime.TryParse(chirp.CreatedDate, out var parsedDate))
            {
                parsedDate = DateTime.Now;
            }

            
            var cheep = new Cheep
            {
                AuthorId = author.Id,
                Text = chirp.Message,
                TimeStamp = parsedDate
            };
            
            author.Cheeps.Add(cheep);
            _context.Cheeps.Add(cheep);
            _context.SaveChanges();
        }


        private readonly Expression<Func<Cheep, CheepDTO>> createCheepDTO =
            c => new CheepDTO
            {
                Author = c.Author.Name,
                Message = c.Text,
                CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
            };

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
