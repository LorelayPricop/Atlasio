using backend.Infrastructure.Data;
using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace backend.Infrastructure.Services
{
    /// <summary>
    /// Servicio responsable de poblar la base de datos con datos de ejemplo
    /// Se ejecuta automáticamente al iniciar la aplicación si la base está vacía
    /// </summary>
    public class DataSeedService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">Contexto de Entity Framework para insertar datos</param>
        public DataSeedService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método principal que ejecuta el seed de datos
        /// Solo se ejecuta si no hay destinos en la base de datos
        /// Orden: Countries → DestinationTypes → Destinations
        /// </summary>
        public async Task SeedDataAsync()
        {
            Log.Information("Starting data seed process");
            
            // Verificar si ya existen destinos para evitar duplicación
            if (_context.Destinations.Any())
            {
                Log.Information("Database already contains destinations. Skipping data seed");
                return;
            }

            Log.Information("Empty database. Starting sample data seed");

            // 1. Seed Countries (no dependencies)
            SeedCountriesAsync();
            await _context.SaveChangesAsync();
            Log.Information("Countries seeded successfully");

            // 2. Seed DestinationTypes (no dependencies)
            SeedDestinationTypesAsync();
            await _context.SaveChangesAsync();
            Log.Information("Destination types seeded successfully");

            // 3. Seed Destinations (depends on Countries and DestinationTypes)
            await SeedDestinationsAsync();
            await _context.SaveChangesAsync();
            
            Log.Information("Data seed completed successfully");
        }

        /// <summary>
        /// Puebla la tabla Countries con datos de ejemplo
        /// </summary>
        private void SeedCountriesAsync()
        {
            var countries = new List<Country>
            {
                new() { Code = "MEX", Name = "Mexico", Region = "North America" },
                new() { Code = "USA", Name = "United States", Region = "North America" },
                new() { Code = "ESP", Name = "Spain", Region = "Europe" },
                new() { Code = "FRA", Name = "France", Region = "Europe" },
                new() { Code = "GRC", Name = "Greece", Region = "Europe" },
                new() { Code = "JPN", Name = "Japan", Region = "Asia" },
                new() { Code = "PER", Name = "Peru", Region = "South America" },
                new() { Code = "BRA", Name = "Brazil", Region = "South America" },
                new() { Code = "IDN", Name = "Indonesia", Region = "Asia" },
                new() { Code = "CHE", Name = "Switzerland", Region = "Europe" }
            };
            
            _context.Countries.AddRange(countries);
        }

        /// <summary>
        /// Puebla la tabla DestinationTypes con los tipos disponibles
        /// </summary>
        private void SeedDestinationTypesAsync()
        {
            var types = new List<DestinationType>
            {
                new() 
                { 
                    Code = "BEACH", 
                    Name = "Beach", 
                    Icon = "beach_access", 
                    DisplayOrder = 1,
                    ColorBackground = "#dbeafe",
                    ColorForeground = "#0284c7"
                },
                new() 
                { 
                    Code = "MOUNTAIN", 
                    Name = "Mountain", 
                    Icon = "terrain", 
                    DisplayOrder = 2,
                    ColorBackground = "#d1fae5",
                    ColorForeground = "#059669"
                },
                new() 
                { 
                    Code = "CITY", 
                    Name = "City", 
                    Icon = "location_city", 
                    DisplayOrder = 3,
                    ColorBackground = "#e0e7ff",
                    ColorForeground = "#4f46e5"
                },
                new() 
                { 
                    Code = "CULTURAL", 
                    Name = "Cultural", 
                    Icon = "museum", 
                    DisplayOrder = 4,
                    ColorBackground = "#fae8ff",
                    ColorForeground = "#a855f7"
                },
                new() 
                { 
                    Code = "ADVENTURE", 
                    Name = "Adventure", 
                    Icon = "hiking", 
                    DisplayOrder = 5,
                    ColorBackground = "#fed7aa",
                    ColorForeground = "#ea580c"
                },
                new() 
                { 
                    Code = "RELAX", 
                    Name = "Relax", 
                    Icon = "spa", 
                    DisplayOrder = 6,
                    ColorBackground = "#ccfbf1",
                    ColorForeground = "#14b8a6"
                }
            };

            _context.DestinationTypes.AddRange(types);
        }

        /// <summary>
        /// Puebla la tabla Destinations con destinos de ejemplo
        /// Requiere que Countries y DestinationTypes ya estén seeded
        /// </summary>
        private async Task SeedDestinationsAsync()
        {
            // Obtener los IDs de los tipos de destino
            var types = await _context.DestinationTypes.ToListAsync();
            var beachId = types.First(t => t.Code == "BEACH").Id;
            var culturalId = types.First(t => t.Code == "CULTURAL").Id;
            var cityId = types.First(t => t.Code == "CITY").Id;
            var adventureId = types.First(t => t.Code == "ADVENTURE").Id;
            var mountainId = types.First(t => t.Code == "MOUNTAIN").Id;
            var relaxId = types.First(t => t.Code == "RELAX").Id;

            var destinations = new List<Destination>
            {
                new()
                {
                    Name = "Playa del Carmen",
                    Description = "Beautiful Caribbean beach with crystal-clear waters and white sand. Perfect for diving and snorkeling.",
                    LongDescription = "Playa del Carmen is one of the most popular destinations in the Mexican Riviera Maya. With its white sand beaches, turquoise waters and coral reefs, it offers unforgettable experiences for both diving enthusiasts and those seeking to relax under the Caribbean sun. Fifth Avenue, its main street, is full of restaurants, shops and nightlife. Additionally, it is the perfect starting point to explore Tulum, Cozumel and nearby cenotes.",
                    CountryCode = "MEX",
                    DestinationTypeId = beachId,
                    ImageUrl = "https://images.unsplash.com/photo-1512813389649-e0c5bb4c6a51",
                    TotalBookings = 1250,
                    AverageRating = 4.7m,
                    ReviewCount = 892,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-30),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Santorini",
                    Description = "Greek island famous for its white and blue houses, spectacular sunsets and Aegean Sea views.",
                    LongDescription = "Santorini is the jewel of the Greek Cyclades islands, known worldwide for its breathtaking sunsets in Oia, its whitewashed houses with blue domes perched on volcanic cliffs, and its unique red and black sand beaches. The island offers a perfect combination of natural beauty, ancient history (with the Minoan ruins of Akrotiri), exquisite Mediterranean cuisine and distinctive local wines grown in volcanic vineyards.",
                    CountryCode = "GRC",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1613395877344-13d4a8e0d49e",
                    TotalBookings = 2100,
                    AverageRating = 4.9m,
                    ReviewCount = 1534,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-45),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Kyoto",
                    Description = "Former capital of Japan with historic temples, Zen gardens and the famous geisha of Gion.",
                    LongDescription = "Kyoto was the imperial capital of Japan for over a thousand years and retains traditional Japanese charm with its 2,000 Buddhist temples, Shinto shrines and meticulously maintained Zen gardens. Walking through the Gion district at dusk may reveal the charm of geishas, while the Arashiyama Bamboo Forest offers a magical experience. During spring, the cherry blossom (sakura) turns the city into a pink spectacle, and in autumn, maple trees offer golden and red hues.",
                    CountryCode = "JPN",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e",
                    TotalBookings = 1875,
                    AverageRating = 4.8m,
                    ReviewCount = 1267,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-60),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Machu Picchu",
                    Description = "Lost Inca city high in the Peruvian Andes, one of the wonders of the world.",
                    LongDescription = "Machu Picchu, the legendary Inca citadel located 2,430 meters above sea level, is considered one of the Seven Wonders of the Modern World. Built in the 15th century and rediscovered in 1911, this impressive stone city is nestled between green mountains and clouds, offering spectacular views of the Sacred Valley. Visitors can arrive via the famous Inca Trail (a 4-day hike) or by train from Cusco, and explore its agricultural terraces, temples, astronomical observatories and the mysterious Intihuatana (ritual stone).",
                    CountryCode = "PER",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1587595431973-160d0d94add1",
                    TotalBookings = 1650,
                    AverageRating = 4.95m,
                    ReviewCount = 2103,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-90),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Paris",
                    Description = "The city of love, with the Eiffel Tower, the Louvre and the Champs-Élysées.",
                    LongDescription = "Paris, the City of Light, captivates millions of visitors each year with its unparalleled blend of history, art, gastronomy and romance. From the iconic Eiffel Tower to the masterpieces of the Louvre Museum (including the Mona Lisa), through the majestic Notre-Dame Cathedral and the bohemian Montmartre neighborhood, every corner tells a story. Strolling down the Champs-Élysées, sailing on the Seine, savoring croissants on a café terrace or exploring the Latin Quarter are experiences that define Parisian essence.",
                    CountryCode = "FRA",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34",
                    TotalBookings = 3200,
                    AverageRating = 4.85m,
                    ReviewCount = 2845,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-120),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "New York",
                    Description = "The city that never sleeps, with Times Square, Central Park and the Statue of Liberty.",
                    LongDescription = "New York is the cultural, financial and entertainment epicenter of the world. The Big Apple offers unique experiences: from gazing at the Statue of Liberty and Manhattan's skyline, to getting lost in the 341 hectares of Central Park, watching a Broadway musical, visiting world-class museums (MET, MoMA, Guggenheim), or enjoying the vibrant energy of Times Square. The five boroughs (Manhattan, Brooklyn, Queens, Bronx and Staten Island) offer incomparable cultural diversity with neighborhoods like Chinatown, Little Italy, SoHo and Williamsburg.",
                    CountryCode = "USA",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1496442226666-8d4d0e62e6e9",
                    TotalBookings = 4150,
                    AverageRating = 4.75m,
                    ReviewCount = 3521,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-100),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Barcelona",
                    Description = "Catalan city with Gaudí's architecture, Mediterranean beaches and the Sagrada Familia.",
                    LongDescription = "Barcelona is a vibrant Mediterranean metropolis that masterfully combines its rich historical heritage with a unique modernist spirit. The city is famous for Antoni Gaudí's surrealist architectural works, especially the Sagrada Familia Basilica (still under construction), Park Güell and Casa Batlló. Las Ramblas, the Gothic Quarter, Barceloneta beach, La Boquería market and Camp Nou (FC Barcelona stadium) are must-see stops. Catalan gastronomy, with its tapas and paellas, perfectly complements the experience.",
                    CountryCode = "ESP",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1583422409516-2895a77efded",
                    TotalBookings = 2650,
                    AverageRating = 4.8m,
                    ReviewCount = 1987,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-75),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Rio de Janeiro",
                    Description = "Brazilian city famous for Christ the Redeemer, Sugarloaf Mountain and Copacabana beaches.",
                    LongDescription = "Rio de Janeiro, the 'Cidade Maravilhosa', is a dazzling destination where green mountains meet golden beaches and the Atlantic Ocean. Christ the Redeemer on Corcovado and Sugarloaf Mountain offer spectacular panoramic views of the city. The famous beaches of Copacabana and Ipanema are the heart of the Carioca lifestyle, while Rio Carnival is the largest and most colorful celebration on the planet. The urban jungle of Tijuca Forest, the neighborhoods of Santa Teresa and Lapa, and samba complete the Brazilian experience.",
                    CountryCode = "BRA",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1483729558449-99ef09a8c325",
                    TotalBookings = 1890,
                    AverageRating = 4.65m,
                    ReviewCount = 1456,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-50),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Swiss Alps",
                    Description = "Impressive mountains for skiing, hiking and winter sports.",
                    LongDescription = "The Swiss Alps represent the ultimate paradise for mountain lovers and winter sports enthusiasts. With iconic peaks like the Matterhorn, Jungfrau and Eiger, the region offers world-class ski resorts such as Zermatt, St. Moritz and Verbier. In summer, the mountains transform into spectacular hiking trails with alpine meadows full of flowers, crystal-clear lakes and picturesque villages. Panoramic trains like the Glacier Express offer breathtaking views of the alpine landscape.",
                    CountryCode = "CHE",
                    DestinationTypeId = mountainId,
                    ImageUrl = "https://images.unsplash.com/photo-1531366936337-7c912a4589a7",
                    TotalBookings = 1425,
                    AverageRating = 4.9m,
                    ReviewCount = 978,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-110),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Bali",
                    Description = "Indonesian island famous for its temples, beaches and relaxing atmosphere.",
                    LongDescription = "Bali, the 'Island of the Gods', is a tropical paradise that combines spirituality, natural beauty and Balinese hospitality. With sacred temples like Tanah Lot and Uluwatu perched on cliffs, emerald rice terraces in Tegalalang, surf beaches in Uluwatu and Canggu, and the cultural center of Ubud with its art galleries and traditional markets, Bali offers unique experiences. Yoga retreats, luxury spas, traditional ceremonies and delicious Indonesian cuisine make this island the perfect destination for relaxation and spiritual rejuvenation.",
                    CountryCode = "IDN",
                    DestinationTypeId = relaxId,
                    ImageUrl = "https://images.unsplash.com/photo-1537996194471-e657df975ab4",
                    TotalBookings = 2340,
                    AverageRating = 4.85m,
                    ReviewCount = 1723,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-35),
                    LastModif = DateTime.UtcNow
                }
            };
            
            _context.Destinations.AddRange(destinations);
        }
    }
}
