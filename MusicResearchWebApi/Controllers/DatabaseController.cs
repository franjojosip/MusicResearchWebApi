using MusicResearchWebApi.DatabaseModels;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MusicResearchWebApi.Controllers
{
    [RoutePrefix("db")]
    public class DatabaseController : ApiController
    {
        private readonly string connectionString = ConfigurationManager.AppSettings["dbConnectionString"];

        [HttpGet]
        [Route("genres", Name="Get-Genres")]
        public HttpResponseMessage GetGenres()
        {
            List<Genre> genres = new List<Genre>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM Genre";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                genres.Add(new Genre() { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, genres);
        }

        [HttpGet]
        [Route("songs", Name="Get-Songs")]
        public HttpResponseMessage GetSongs()
        {
            List<SongRestModel> songs = new List<SongRestModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"SELECT Song.Id, Song.Name, Genre.Name FROM Song JOIN Genre ON Genre.Id = Song.GenreId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                songs.Add(new SongRestModel() { Id = reader.GetString(0), Name = reader.GetString(1), Genre = reader.GetString(2) });
                            }
                        } 
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, songs);
        }

        [HttpGet]
        [Route("", Name="Song-Find-By-Search-Phrase")]
        public HttpResponseMessage FindBySearchPhrase([FromUri]string searchPhrase)
        {
            List<SongRestModel> songs = new List<SongRestModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"SELECT Song.Id, Song.Name, Genre.Name FROM Song JOIN Genre ON Genre.Id = Song.GenreId WHERE Song.Name LIKE '%{searchPhrase}%'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                songs.Add(new SongRestModel() { Id = reader.GetString(0), Name = reader.GetString(1), Genre = reader.GetString(2) });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, songs);
        }

        [HttpGet]
        [Route("genre", Name = "Genre-Get-By-Name")]
        public HttpResponseMessage GetGenre([FromUri]string name)
        {
            Genre genre = new Genre();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"SELECT Id, Name FROM Genre WHERE Name like '{name}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                genre.Id = reader.GetInt32(0);
                                genre.Name = reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, genre);
        }

        [HttpPut]
        [Route("{id}", Name="Song-Put" )]
        public HttpResponseMessage UpdateSong([FromBody]Song model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"Update Song SET Song.Name = '{model.Name}' WHERE Song.Id = '{model.Id}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("{id}", Name="Song-Delete")]
        public HttpResponseMessage DeleteSong(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"DELETE FROM Song WHERE Song.Id = '{id}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("", Name = "Song-Post")]
        public HttpResponseMessage PostSong([FromBody]Song model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO Song (Id, Name, GenreId, DateCreated) VALUES (newid(), '{model.Name}', {model.GenreId}, getdate())";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public class SongRestModel
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Genre { get; set; }
        }



    }
}
