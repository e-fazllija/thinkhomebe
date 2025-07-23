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

            // Prima crea la provincia di Roma
            var romaProvince = new Province { Name = "Roma" };
            context.Provinces.Add(romaProvince);
            await context.SaveChangesAsync();

            // Crea le città
            var cities = new List<City>
            {
                new City { Name = "Ardea", ProvinceId = romaProvince.Id },
                new City { Name = "Cave", ProvinceId = romaProvince.Id },
                new City { Name = "Fiumicino", ProvinceId = romaProvince.Id },
                new City { Name = "Genazzano", ProvinceId = romaProvince.Id },
                new City { Name = "Guidonia Montecelio", ProvinceId = romaProvince.Id },
                new City { Name = "Labico", ProvinceId = romaProvince.Id },
                new City { Name = "Monte Compatri", ProvinceId = romaProvince.Id },
                new City { Name = "Palestrina", ProvinceId = romaProvince.Id },
                new City { Name = "Rocca Priora", ProvinceId = romaProvince.Id },
                new City { Name = "Roma", ProvinceId = romaProvince.Id },
                new City { Name = "Tivoli", ProvinceId = romaProvince.Id },
                new City { Name = "Zagarolo", ProvinceId = romaProvince.Id }
            };

            context.Cities.AddRange(cities);
            await context.SaveChangesAsync();

            // Crea le località
            var locations = new List<Location>
            {
                // Ardea
                new Location { Name = "Centro Storico", CityId = cities.First(c => c.Name == "Ardea").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Nuova California", CityId = cities.First(c => c.Name == "Ardea").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor San Lorenzo", CityId = cities.First(c => c.Name == "Ardea").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Marina di Ardea", CityId = cities.First(c => c.Name == "Ardea").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castagnetta", CityId = cities.First(c => c.Name == "Ardea").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Cave
                new Location { Name = "Colle Palme", CityId = cities.First(c => c.Name == "Cave").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Bartolomeo", CityId = cities.First(c => c.Name == "Cave").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Fiumicino
                new Location { Name = "Fiumicino Paese", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Isola Sacra", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Focene", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fregene", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Palidoro", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Parco Leonardo", CityId = cities.First(c => c.Name == "Fiumicino").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Genazzano
                new Location { Name = "Centro Storico", CityId = cities.First(c => c.Name == "Genazzano").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Vito", CityId = cities.First(c => c.Name == "Genazzano").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Pizzuto", CityId = cities.First(c => c.Name == "Genazzano").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Sfondato", CityId = cities.First(c => c.Name == "Genazzano").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Guidonia Montecelio
                new Location { Name = "Albuccione", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casacalda", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castel Arcione", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Verde", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Botte", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Marco Simone", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Montecelio", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Setteville", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Nova", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villalba", CityId = cities.First(c => c.Name == "Guidonia Montecelio").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Labico
                new Location { Name = "Colle Spina", CityId = cities.First(c => c.Name == "Labico").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Monte Compatri
                new Location { Name = "Colle Mattia", CityId = cities.First(c => c.Name == "Monte Compatri").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Laghetto", CityId = cities.First(c => c.Name == "Monte Compatri").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Palestrina
                new Location { Name = "Carchitti", CityId = cities.First(c => c.Name == "Palestrina").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Valvarino", CityId = cities.First(c => c.Name == "Palestrina").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Rocca Priora
                new Location { Name = "Colle di Fuori", CityId = cities.First(c => c.Name == "Rocca Priora").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Roma
                new Location { Name = "Acilia", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Alessandrino", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Axa", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Boccea", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Borghesiana", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casal Bernocchi", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casal Palocco", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Casalotti", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castel Fusano", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Castelverde", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Centocelle", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Centro Giano", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Cinecittà", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Monfortani", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle Prenestino", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle del Sole", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Corcolle", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Corcolle Alto", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Don Bosco", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Dragona", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Dragoncello", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Eur", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Finocchio", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fonte Laurentina", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Fosso San Giuliano", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Giardinetti", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Giardini di Corcolle", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Infernetto", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Pisana", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "La Storta", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Longarina", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Lunghezza", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Madonnetta", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Malafede", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Monte Sacro", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Osteria del Curato", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Antica", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Levante", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ostia Ponente", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Pantano Borghese", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Piana del Sole", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Pigneto", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Galeria", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte di Nona", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Portuense", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Prato Fiorito", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Prima Porta", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Rocca Cencia", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Romanina", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Roma Est", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Vittorino", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Saxa Rubra", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Settecamini", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Bella Monaca", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Tre Teste", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor Vergata", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tor de Cenci", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Angela", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Gaia", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Maura", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Torre Spaccata", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tuscolana", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Vallerano", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Vermicino", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Verde", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villaggio Breda", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villaggio Prenestino", CityId = cities.First(c => c.Name == "Roma").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Tivoli
                new Location { Name = "Centro Storico", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Tivoli Terme", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Villa Adriana", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Campolimpido", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Colle di Tivoli", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Ponte Lucano", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "San Paolo", CityId = cities.First(c => c.Name == "Tivoli").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },

                // Zagarolo
                new Location { Name = "Centro Paese", CityId = cities.First(c => c.Name == "Zagarolo").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Valle Martella", CityId = cities.First(c => c.Name == "Zagarolo").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now },
                new Location { Name = "Via Prenestina", CityId = cities.First(c => c.Name == "Zagarolo").Id, CreationDate = DateTime.Now, UpdateDate = DateTime.Now }
            };

            context.Locations.AddRange(locations);
            await context.SaveChangesAsync();
        }
    }
} 