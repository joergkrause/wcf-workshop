using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Seminar.DataSource {

  [DataContract(Name = "U")]
  [Table("Customers")]
  public class Customer {


    public int Id { get; set; }

    [DataMember(Name = "N", Order = 10)]
    [Column("Name")]
    [StringLength(100)]  // nvarchar(100)
    [Required]           // NOT NULL 
    public string UserName { get; set; }

    [DataMember(Name = "R", Order = 20)]
    [StringLength(10)]
    public string Room { get; set; }

    [DataMember(Name = "P", Order = 30)]
    [StringLength(20)]
    public string Password { get; set; }

    [NotMapped]
    public string HashedPassword {
      get {
        return String.Join(String.Empty, Password.ToArray().Reverse());
      }
    }

  }
}
