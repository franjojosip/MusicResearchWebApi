using System;

namespace MusicResearchWebApi.DatabaseModels
{
    public class Song
    {
        public Int32 Id { get; set; }

        public string Name { get; set; }

        public int GenreId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}