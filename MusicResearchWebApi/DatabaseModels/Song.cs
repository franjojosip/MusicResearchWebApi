using System;

namespace MusicResearchWebApi.DatabaseModels
{
    public class Song
    {
        public Int32 Id { get; set; }

        public string Name { get; set; }

        public Int32 GenreId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}