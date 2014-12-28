using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKienThao.Domain.Models
{
    public class DictionaryContext : DbContext
    {
        public DictionaryContext() : base("DictionaryJaConnection") { }
        public DictionaryContext(string connection) : base(connection) { }
        public DbSet<WordModel> Words { get; set; }
        public DbSet<KanjiModel> Kanjis { get; set; }
    }
}
