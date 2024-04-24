using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using worldCitiesServer.Data.Models;

namespace worldCitiesServer.Data
{
    public class CityDTO
    {

        public int Id { get; set; }

        public required string Name { get; set; }


        public decimal Lat { get; set; }


        public decimal Lon { get; set; }


        public int CountryId { get; set; }


        public string? CountryName { get; set; }
    }
}
