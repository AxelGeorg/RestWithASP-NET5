using RestWithASPNETUdemy.Model.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestWithASPNETUdemy.Data.VO
{
    public class BookVO
    {
        public long Id { get; set; }

        public decimal Price { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime LaunchDate { get; set; }
    }
}
