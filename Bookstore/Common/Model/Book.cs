using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Book : ICloneable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }
        public string Username { get; set; }
        [ForeignKey("Username")]
        public Member Member { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public DateTime LastModified { get; set; }

        public ICloneable Clone()
        {
            Book clone = new Book();
            clone.Title = Title;
            clone.PublishYear = PublishYear;
            clone.AuthorId = AuthorId;
            clone.LastModified = DateTime.Now;

            return clone;
        }
    }
}
