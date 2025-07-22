using BackEnd.Data;
using BackEnd.Entities;

namespace BackEnd.Services
{
    public static class LocationDataSeeder
    {
        public static async Task SeedLocations(AppDbContext context)
        {
            if (context.Locations.Any())
                return; // Database already seeded

            var locations = new List<Location>
            {
                // Ardea
                new Location { Name = "Centro Storico", City = "Ardea", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Nuova California", City = "Ardea", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor San Lorenzo", City = "Ardea", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Marina di Ardea", City = "Ardea", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castagnetta", City = "Ardea", IsActive = true, OrderIndex = 5, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Cave
                new Location { Name = "Colle Palme", City = "Cave", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Bartolomeo", City = "Cave", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Fiumicino
                new Location { Name = "Fiumicino Paese", City = "Fiumicino", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Isola Sacra", City = "Fiumicino", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Focene", City = "Fiumicino", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fregene", City = "Fiumicino", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Palidoro", City = "Fiumicino", IsActive = true, OrderIndex = 5, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Parco Leonardo", City = "Fiumicino", IsActive = true, OrderIndex = 6, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Genazzano
                new Location { Name = "Centro Storico", City = "Genazzano", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Vito", City = "Genazzano", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Pizzuto", City = "Genazzano", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Sfondato", City = "Genazzano", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Guidonia Montecelio
                new Location { Name = "Albuccione", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casacalda", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castel Arcione", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Verde", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Botte", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 5, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Marco Simone", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 6, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Montecelio", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 7, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Setteville", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 8, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Nova", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 9, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villalba", City = "Guidonia Montecelio", IsActive = true, OrderIndex = 10, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Labico
                new Location { Name = "Colle Spina", City = "Labico", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Monte Compatri
                new Location { Name = "Colle Mattia", City = "Monte Compatri", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Laghetto", City = "Monte Compatri", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Palestrina
                new Location { Name = "Carchitti", City = "Palestrina", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Valvarino", City = "Palestrina", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Rocca Priora
                new Location { Name = "Colle di Fuori", City = "Rocca Priora", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Roma
                new Location { Name = "Acilia", City = "Roma", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Alessandrino", City = "Roma", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Axa", City = "Roma", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Boccea", City = "Roma", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Borghesiana", City = "Roma", IsActive = true, OrderIndex = 5, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casal Bernocchi", City = "Roma", IsActive = true, OrderIndex = 6, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casal Palocco", City = "Roma", IsActive = true, OrderIndex = 7, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casalotti", City = "Roma", IsActive = true, OrderIndex = 8, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castel Fusano", City = "Roma", IsActive = true, OrderIndex = 9, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castelverde", City = "Roma", IsActive = true, OrderIndex = 10, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Centocelle", City = "Roma", IsActive = true, OrderIndex = 11, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Centro Giano", City = "Roma", IsActive = true, OrderIndex = 12, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Cinecittà", City = "Roma", IsActive = true, OrderIndex = 13, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Monfortani", City = "Roma", IsActive = true, OrderIndex = 14, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Prenestino", City = "Roma", IsActive = true, OrderIndex = 15, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle del Sole", City = "Roma", IsActive = true, OrderIndex = 16, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Corcolle", City = "Roma", IsActive = true, OrderIndex = 17, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Corcolle Alto", City = "Roma", IsActive = true, OrderIndex = 18, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Don Bosco", City = "Roma", IsActive = true, OrderIndex = 19, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Dragona", City = "Roma", IsActive = true, OrderIndex = 20, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Dragoncello", City = "Roma", IsActive = true, OrderIndex = 21, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Eur", City = "Roma", IsActive = true, OrderIndex = 22, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Finocchio", City = "Roma", IsActive = true, OrderIndex = 23, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fonte Laurentina", City = "Roma", IsActive = true, OrderIndex = 24, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fosso San Giuliano", City = "Roma", IsActive = true, OrderIndex = 25, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Giardinetti", City = "Roma", IsActive = true, OrderIndex = 26, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Giardini di Corcolle", City = "Roma", IsActive = true, OrderIndex = 27, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Infernetto", City = "Roma", IsActive = true, OrderIndex = 28, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Pisana", City = "Roma", IsActive = true, OrderIndex = 29, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Storta", City = "Roma", IsActive = true, OrderIndex = 30, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Longarina", City = "Roma", IsActive = true, OrderIndex = 31, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Lunghezza", City = "Roma", IsActive = true, OrderIndex = 32, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Madonnetta", City = "Roma", IsActive = true, OrderIndex = 33, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Malafede", City = "Roma", IsActive = true, OrderIndex = 34, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Monte Sacro", City = "Roma", IsActive = true, OrderIndex = 35, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Osteria del Curato", City = "Roma", IsActive = true, OrderIndex = 36, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia", City = "Roma", IsActive = true, OrderIndex = 37, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Antica", City = "Roma", IsActive = true, OrderIndex = 38, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Levante", City = "Roma", IsActive = true, OrderIndex = 39, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Ponente", City = "Roma", IsActive = true, OrderIndex = 40, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Pantano Borghese", City = "Roma", IsActive = true, OrderIndex = 41, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Piana del Sole", City = "Roma", IsActive = true, OrderIndex = 42, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Pigneto", City = "Roma", IsActive = true, OrderIndex = 43, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Galeria", City = "Roma", IsActive = true, OrderIndex = 44, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte di Nona", City = "Roma", IsActive = true, OrderIndex = 45, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Portuense", City = "Roma", IsActive = true, OrderIndex = 46, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Prato Fiorito", City = "Roma", IsActive = true, OrderIndex = 47, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Prima Porta", City = "Roma", IsActive = true, OrderIndex = 48, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Rocca Cencia", City = "Roma", IsActive = true, OrderIndex = 49, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Romanina", City = "Roma", IsActive = true, OrderIndex = 50, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Roma Est", City = "Roma", IsActive = true, OrderIndex = 51, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Vittorino", City = "Roma", IsActive = true, OrderIndex = 52, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Saxa Rubra", City = "Roma", IsActive = true, OrderIndex = 53, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Settecamini", City = "Roma", IsActive = true, OrderIndex = 54, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Bella Monaca", City = "Roma", IsActive = true, OrderIndex = 55, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Tre Teste", City = "Roma", IsActive = true, OrderIndex = 56, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Vergata", City = "Roma", IsActive = true, OrderIndex = 57, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor de Cenci", City = "Roma", IsActive = true, OrderIndex = 58, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Angela", City = "Roma", IsActive = true, OrderIndex = 59, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Gaia", City = "Roma", IsActive = true, OrderIndex = 60, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Maura", City = "Roma", IsActive = true, OrderIndex = 61, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Spaccata", City = "Roma", IsActive = true, OrderIndex = 62, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tuscolana", City = "Roma", IsActive = true, OrderIndex = 63, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Vallerano", City = "Roma", IsActive = true, OrderIndex = 64, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Vermicino", City = "Roma", IsActive = true, OrderIndex = 65, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Verde", City = "Roma", IsActive = true, OrderIndex = 66, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villaggio Breda", City = "Roma", IsActive = true, OrderIndex = 67, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villaggio Prenestino", City = "Roma", IsActive = true, OrderIndex = 68, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Tivoli
                new Location { Name = "Centro Storico", City = "Tivoli", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tivoli Terme", City = "Tivoli", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Adriana", City = "Tivoli", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Campolimpido", City = "Tivoli", IsActive = true, OrderIndex = 4, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle di Tivoli", City = "Tivoli", IsActive = true, OrderIndex = 5, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Lucano", City = "Tivoli", IsActive = true, OrderIndex = 6, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Paolo", City = "Tivoli", IsActive = true, OrderIndex = 7, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Zagarolo
                new Location { Name = "Centro Paese", City = "Zagarolo", IsActive = true, OrderIndex = 1, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Valle Martella", City = "Zagarolo", IsActive = true, OrderIndex = 2, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Via Prenestina", City = "Zagarolo", IsActive = true, OrderIndex = 3, CreationDate = DateTime.Now, UpdateDate = DateTime.Now }
            };

            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
        }
    }
} 