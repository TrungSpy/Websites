using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKienThao.Domain.Models
{
    public class KanjiModel
    {
        public KanjiModel()
        {
            Words = new HashSet<WordModel>();
        }
        [Key]
        public int Id { get; set; }
        [Index(IsUnique=true)]
        [MaxLength(1)]
        [DisplayName("Kanji")]
        public string Name { get; set; }
        [Index]
        [MaxLength(200)]
        [DisplayName("Kana")]
        public string SoundJa { get; set; }
        [Index]
        [MaxLength(200)]
        public string SoundRo { get; set; }
        [Index]
        [MaxLength(50)]
        [DisplayName("Han Viet")]
        public string SoundVn { get; set; }
        [Index]
        [MaxLength(200)]
        [DisplayName("English")]
        public string MeaningEn { get; set; }
        [Index]
        [MaxLength(200)]
        [DisplayName("Vietnamese")]
        public string MeaningVi { get; set; }
        public virtual ICollection<WordModel> Words {get; set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool InSoundJa(string str)
        {
            return SoundJa.Split(',').Contains(str) || SoundRo.Split(',').Contains(str);
        }

        public bool InSoundVn(string str)
        {
            return SoundVn.Split(',').Contains(str);
        }
    }

    public class WordModel{
        public WordModel()
        {
            Kanjis = new HashSet<KanjiModel>();
        }
        [Key]
        public int Id { get; set; }
        [Index]
        [MaxLength(50)]
        public string Name { get; set; }
        public string SoundJa { get; set; }
        public string MeaningEn { get; set; }
        public virtual ICollection<KanjiModel> Kanjis { get; set; }
    }
}
