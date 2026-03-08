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
        /// Genera una URL de imagen confiable usando el API de Unsplash Source
        /// </summary>
        /// <param name="keyword">Palabra clave para la búsqueda de imagen</param>
        /// <param name="width">Ancho de la imagen (default: 800)</param>
        /// <param name="height">Alto de la imagen (default: 600)</param>
        /// <returns>URL de imagen de Unsplash</returns>
        private string GetImageUrl(string keyword, int width = 800, int height = 600)
        {
            // Usar Unsplash Source API que es más confiable
            return $"https://source.unsplash.com/{width}x{height}/?{keyword}";
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
                // América del Norte
                new() { Code = "MEX", Name = "Mexico", Region = "North America" },
                new() { Code = "USA", Name = "United States", Region = "North America" },
                new() { Code = "CAN", Name = "Canada", Region = "North America" },

                // América del Sur
                new() { Code = "PER", Name = "Peru", Region = "South America" },
                new() { Code = "BRA", Name = "Brazil", Region = "South America" },
                new() { Code = "ARG", Name = "Argentina", Region = "South America" },
                new() { Code = "CHL", Name = "Chile", Region = "South America" },
                new() { Code = "ECU", Name = "Ecuador", Region = "South America" },

                // Europa
                new() { Code = "ESP", Name = "Spain", Region = "Europe" },
                new() { Code = "FRA", Name = "France", Region = "Europe" },
                new() { Code = "ITA", Name = "Italy", Region = "Europe" },
                new() { Code = "GRC", Name = "Greece", Region = "Europe" },
                new() { Code = "CHE", Name = "Switzerland", Region = "Europe" },
                new() { Code = "GBR", Name = "United Kingdom", Region = "Europe" },
                new() { Code = "CZE", Name = "Czech Republic", Region = "Europe" },
                new() { Code = "HRV", Name = "Croatia", Region = "Europe" },
                new() { Code = "DNK", Name = "Denmark", Region = "Europe" },
                new() { Code = "ISL", Name = "Iceland", Region = "Europe" },
                new() { Code = "PRT", Name = "Portugal", Region = "Europe" },

                // Asia
                new() { Code = "JPN", Name = "Japan", Region = "Asia" },
                new() { Code = "IDN", Name = "Indonesia", Region = "Asia" },
                new() { Code = "THA", Name = "Thailand", Region = "Asia" },
                new() { Code = "KHM", Name = "Cambodia", Region = "Asia" },
                new() { Code = "ARE", Name = "United Arab Emirates", Region = "Asia" },

                // África
                new() { Code = "MAR", Name = "Morocco", Region = "Africa" },
                new() { Code = "SYC", Name = "Seychelles", Region = "Africa" },

                // Oceanía
                new() { Code = "NZL", Name = "New Zealand", Region = "Oceania" },
                new() { Code = "FJI", Name = "Fiji", Region = "Oceania" },

                // Medio Oriente
                new() { Code = "JOR", Name = "Jordan", Region = "Middle East" }
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
                },
                // 30 destinos adicionales
                new()
                {
                    Name = "Cancún",
                    Description = "Mexican Caribbean paradise with turquoise waters and Mayan ruins nearby.",
                    LongDescription = "Cancún is Mexico's premier beach destination, offering pristine white sand beaches, crystal-clear turquoise waters, and vibrant nightlife. Beyond the beaches, visitors can explore ancient Mayan ruins at Chichén Itzá and Tulum, swim in cenotes (natural sinkholes), and enjoy world-class resorts. The hotel zone features luxury accommodations, while downtown Cancún provides authentic Mexican culture and cuisine.",
                    CountryCode = "MEX",
                    DestinationTypeId = beachId,
                    ImageUrl = "https://images.unsplash.com/photo-1568402102990-bc541580b59f?w=800&h=600&fit=crop&auto=format&q=80",
                    TotalBookings = 2890,
                    AverageRating = 4.7m,
                    ReviewCount = 2156,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-125),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Tokyo",
                    Description = "Japan's capital blending ultra-modern technology with traditional temples.",
                    LongDescription = "Tokyo is a mesmerizing metropolis where ancient traditions and cutting-edge technology coexist harmoniously. From the serene Senso-ji Temple and Imperial Palace to the neon-lit streets of Shibuya and futuristic Odaiba, Tokyo offers endless discoveries. Experience world-class sushi at Tsukiji Market, shop in trendy Harajuku, enjoy panoramic views from Tokyo Tower, and witness the organized chaos of the world's busiest intersection at Shibuya Crossing.",
                    CountryCode = "JPN",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800&h=600&fit=crop&auto=format&q=80",
                    TotalBookings = 3450,
                    AverageRating = 4.8m,
                    ReviewCount = 2834,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-95),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Maldives",
                    Description = "Tropical paradise with overwater bungalows and crystal-clear lagoons.",
                    LongDescription = "The Maldives is the ultimate luxury beach destination, consisting of 26 coral atolls with pristine white sand beaches and luminescent blue waters. Famous for its overwater bungalows, world-class diving and snorkeling, and unparalleled marine biodiversity, the Maldives offers complete privacy and relaxation. Each resort typically occupies its own island, providing an exclusive tropical paradise experience with exceptional service.",
                    CountryCode = "IDN",
                    DestinationTypeId = relaxId,
                    ImageUrl = "https://images.unsplash.com/photo-1514282401047-d79a71a590e8",
                    TotalBookings = 1567,
                    AverageRating = 4.95m,
                    ReviewCount = 1234,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-140),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Grand Canyon",
                    Description = "Massive natural wonder carved by the Colorado River over millions of years.",
                    LongDescription = "The Grand Canyon is one of the world's most spectacular natural wonders, stretching 277 miles long and up to 18 miles wide. This UNESCO World Heritage site offers breathtaking views from multiple viewpoints on the South and North Rim. Adventure seekers can hike into the canyon, raft the Colorado River, or take helicopter tours for aerial perspectives of this geological masterpiece that reveals two billion years of Earth's history.",
                    CountryCode = "USA",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1474044159687-1ee9f3a51722",
                    TotalBookings = 2234,
                    AverageRating = 4.85m,
                    ReviewCount = 1876,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-105),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Amalfi Coast",
                    Description = "Stunning Italian coastline with colorful cliffside villages.",
                    LongDescription = "The Amalfi Coast is a 50-kilometer stretch of coastline in southern Italy renowned for its dramatic beauty. Pastel-colored villages like Positano, Amalfi, and Ravello cling to steep cliffs overlooking the turquoise Mediterranean. Winding coastal roads offer spectacular views, while lemon groves produce the famous Limoncello liqueur. The combination of Italian cuisine, historic villas, beautiful beaches, and warm hospitality makes it a dream destination.",
                    CountryCode = "ITA",
                    DestinationTypeId = beachId,
                    ImageUrl = "https://images.unsplash.com/photo-1534113414509-0bd0019d46ee",
                    TotalBookings = 1789,
                    AverageRating = 4.9m,
                    ReviewCount = 1456,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-88),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Iceland",
                    Description = "Land of fire and ice with glaciers, volcanoes, and Northern Lights.",
                    LongDescription = "Iceland is a geological wonderland where fire and ice create surreal landscapes. Witness the dancing Northern Lights, relax in the Blue Lagoon geothermal spa, explore ice caves in Vatnajökull glacier, and marvel at powerful waterfalls like Gullfoss and Skógafoss. The Golden Circle tour includes Þingvellir National Park, Geysir geothermal area, and stunning natural phenomena. Summer's midnight sun and winter's aurora borealis offer unique seasonal experiences.",
                    CountryCode = "ISL",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1504893524553-b855bce32c67",
                    TotalBookings = 1456,
                    AverageRating = 4.92m,
                    ReviewCount = 1123,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-115),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Dubai",
                    Description = "Futuristic city with world's tallest building and luxury shopping.",
                    LongDescription = "Dubai is a dazzling city of superlatives in the Arabian Desert. Home to the world's tallest building (Burj Khalifa), largest shopping mall (Dubai Mall), and stunning artificial Palm Islands, Dubai combines traditional Arabian culture with ultra-modern luxury. Experience desert safaris, indoor skiing, gold souks, spice markets, and some of the world's most luxurious hotels. The city's tax-free shopping and year-round sunshine attract millions of visitors.",
                    CountryCode = "ARE",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1512453979798-5ea266f8880c",
                    TotalBookings = 2678,
                    AverageRating = 4.75m,
                    ReviewCount = 2234,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-72),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Machu Picchu Trail",
                    Description = "Ancient Inca trail through cloud forests to the legendary citadel.",
                    LongDescription = "The Inca Trail to Machu Picchu is one of the world's most famous treks, combining spectacular mountain scenery, lush cloud forests, and fascinating Inca ruins. This 4-day journey follows ancient stone pathways built by the Incas, passing through diverse ecosystems and culminating at the Sun Gate with a breathtaking view of Machu Picchu at sunrise. Limited permits make this an exclusive adventure requiring advance booking.",
                    CountryCode = "PER",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1526392060635-9d6019884377",
                    TotalBookings = 1234,
                    AverageRating = 4.88m,
                    ReviewCount = 987,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-130),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Venice",
                    Description = "Romantic floating city with canals, gondolas, and Renaissance palaces.",
                    LongDescription = "Venice is an incomparable floating city built on 118 islands connected by bridges and canals. Glide through the Grand Canal in a gondola, explore St. Mark's Square and Basilica, visit the colorful island of Burano, and get lost in narrow medieval streets. Famous for its Carnival masks, Murano glass, and Renaissance architecture, Venice offers a timeless romantic atmosphere despite mass tourism challenges.",
                    CountryCode = "ITA",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1523906834658-6e24ef2386f9",
                    TotalBookings = 2456,
                    AverageRating = 4.82m,
                    ReviewCount = 2012,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-98),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Phuket",
                    Description = "Thailand's largest island with stunning beaches and vibrant nightlife.",
                    LongDescription = "Phuket combines beautiful beaches, Thai culture, and modern tourism infrastructure. From the famous Patong Beach with its bustling nightlife to the serene Kata and Karon beaches, the island offers something for everyone. Visit the Big Buddha statue, explore Old Phuket Town's Sino-Portuguese architecture, enjoy fresh seafood, take island-hopping tours to nearby Phi Phi Islands, and experience traditional Thai hospitality.",
                    CountryCode = "THA",
                    DestinationTypeId = beachId,
                    ImageUrl = "https://images.unsplash.com/photo-1589394815804-964ed0be2eb5",
                    TotalBookings = 2123,
                    AverageRating = 4.65m,
                    ReviewCount = 1789,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-67),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Amazon Rainforest",
                    Description = "World's largest tropical rainforest with incredible biodiversity.",
                    LongDescription = "The Amazon Rainforest is Earth's most biodiverse ecosystem, home to millions of species of plants, animals, and indigenous communities. Explore the jungle from lodges in Peru, Brazil, or Ecuador, cruise along the mighty Amazon River, spot pink river dolphins, exotic birds, and monkeys, and learn about medicinal plants from local guides. This adventure offers a profound connection with nature and insights into conservation efforts.",
                    CountryCode = "BRA",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1516026672322-bc52d61a55d5",
                    TotalBookings = 987,
                    AverageRating = 4.78m,
                    ReviewCount = 756,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-145),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Prague",
                    Description = "Fairy-tale European capital with medieval architecture and castle.",
                    LongDescription = "Prague, the 'City of a Hundred Spires', enchants visitors with its well-preserved medieval Old Town, Gothic churches, and baroque buildings. Prague Castle overlooks the city, while the astronomical clock in Old Town Square draws crowds on the hour. Stroll across the historic Charles Bridge, explore the Jewish Quarter, enjoy Czech beer in traditional pubs, and experience the romantic atmosphere that has inspired artists for centuries.",
                    CountryCode = "CZE",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1541849546-216549ae216d",
                    TotalBookings = 1678,
                    AverageRating = 4.76m,
                    ReviewCount = 1345,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-82),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Patagonia",
                    Description = "Pristine wilderness at the southern tip of South America.",
                    LongDescription = "Patagonia spans Argentina and Chile, offering some of the world's most dramatic and unspoiled landscapes. Trek through Torres del Paine National Park, witness the majesty of Perito Moreno Glacier, spot whales in Peninsula Valdés, and experience the raw beauty of the southernmost region of the Americas. This remote destination attracts adventurers seeking wilderness, wildlife, and spectacular mountain scenery.",
                    CountryCode = "CHL",
                    DestinationTypeId = mountainId,
                    ImageUrl = "https://images.unsplash.com/photo-1501594907352-04cda38ebc29",
                    TotalBookings = 876,
                    AverageRating = 4.91m,
                    ReviewCount = 654,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-156),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Marrakech",
                    Description = "Exotic Moroccan city with vibrant souks and historic palaces.",
                    LongDescription = "Marrakech is a sensory explosion of colors, sounds, and aromas. The historic medina features the famous Jemaa el-Fnaa square, bustling souks selling spices and crafts, and architectural marvels like the Koutoubia Mosque and Bahia Palace. The luxurious Majorelle Garden offers a peaceful escape, while traditional riads (houses with interior gardens) provide authentic accommodation. Experience hammam spas, mint tea ceremonies, and aromatic tagines.",
                    CountryCode = "MAR",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1597212618440-806262de4f6b",
                    TotalBookings = 1545,
                    AverageRating = 4.68m,
                    ReviewCount = 1234,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-91),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Yellowstone National Park",
                    Description = "America's first national park with geysers and wildlife.",
                    LongDescription = "Yellowstone National Park is a geothermal wonderland featuring the famous Old Faithful geyser, colorful hot springs like Grand Prismatic Spring, dramatic waterfalls, and the stunning Grand Canyon of Yellowstone. The park is home to grizzly bears, wolves, bison, and elk, offering exceptional wildlife viewing opportunities. Spanning Wyoming, Montana, and Idaho, Yellowstone showcases the power and beauty of nature's geological forces.",
                    CountryCode = "USA",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1490424660805-a403e1e3b6d1",
                    TotalBookings = 1923,
                    AverageRating = 4.83m,
                    ReviewCount = 1567,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-102),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Copenhagen",
                    Description = "Danish capital known for design, cycling culture, and hygge lifestyle.",
                    LongDescription = "Copenhagen exemplifies Scandinavian design, sustainability, and quality of life. Explore colorful Nyhavn harbor, visit the Little Mermaid statue, discover Tivoli Gardens amusement park, and experience New Nordic cuisine at world-class restaurants. The city's extensive bike lanes make cycling the best way to explore. Don't miss Christiania free town, Rosenborg Castle, and the concept of 'hygge' - Danish coziness and contentment.",
                    CountryCode = "DNK",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1513622470522-26c3c8a854bc",
                    TotalBookings = 1456,
                    AverageRating = 4.79m,
                    ReviewCount = 1123,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-78),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Angkor Wat",
                    Description = "Massive ancient temple complex in Cambodia's jungles.",
                    LongDescription = "Angkor Wat is the world's largest religious monument and a stunning example of Khmer architecture from the 12th century. This UNESCO World Heritage site includes hundreds of temples spread across the jungle, with Angkor Wat's five towers being the most iconic. Sunrise over Angkor Wat, exploring the enigmatic faces of Bayon Temple, and discovering jungle-covered Ta Prohm (famous from Tomb Raider) create unforgettable experiences.",
                    CountryCode = "KHM",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1528181304800-259b08848526",
                    TotalBookings = 1234,
                    AverageRating = 4.87m,
                    ReviewCount = 987,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-118),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Fiji Islands",
                    Description = "South Pacific paradise with pristine beaches and coral reefs.",
                    LongDescription = "Fiji consists of over 300 islands offering the ultimate tropical escape. Crystal-clear waters, vibrant coral reefs perfect for snorkeling and diving, and pristine white sand beaches create paradise. Experience authentic Fijian hospitality, enjoy island hopping, visit traditional villages, participate in kava ceremonies, and relax in luxury resorts or budget-friendly hostels. The friendly locals say 'Bula!' (hello/life) embodying the islands' warm spirit.",
                    CountryCode = "FJI",
                    DestinationTypeId = beachId,
                    ImageUrl = "https://images.unsplash.com/photo-1559827260-dc66d52bef19",
                    TotalBookings = 1345,
                    AverageRating = 4.84m,
                    ReviewCount = 1067,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-123),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Petra",
                    Description = "Ancient rose-red city carved into Jordan's desert cliffs.",
                    LongDescription = "Petra is Jordan's most treasured archaeological site and one of the New Seven Wonders of the World. This ancient Nabataean city features elaborate buildings carved directly into rose-red sandstone cliffs, most famously the Treasury (Al-Khazneh). Walk through the narrow Siq canyon, explore the Monastery, visit the Royal Tombs, and imagine life 2,000 years ago in this remarkable trading hub on the ancient Silk Road.",
                    CountryCode = "JOR",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1578070181910-f1e514afdd08",
                    TotalBookings = 967,
                    AverageRating = 4.92m,
                    ReviewCount = 789,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-134),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Banff National Park",
                    Description = "Canadian Rockies paradise with turquoise lakes and mountain peaks.",
                    LongDescription = "Banff National Park in the Canadian Rockies is a year-round outdoor paradise. Marvel at the turquoise waters of Lake Louise and Moraine Lake, surrounded by snow-capped peaks. Hike scenic trails in summer, ski world-class slopes in winter, spot wildlife like elk and bears, and soak in Banff Upper Hot Springs. The charming town of Banff offers excellent restaurants and shops, while the Icefields Parkway provides spectacular mountain vistas.",
                    CountryCode = "CAN",
                    DestinationTypeId = mountainId,
                    ImageUrl = "https://images.unsplash.com/photo-1503614472-8c93d56e92ce",
                    TotalBookings = 1678,
                    AverageRating = 4.89m,
                    ReviewCount = 1345,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-87),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Seychelles",
                    Description = "Exclusive island nation with pristine beaches and unique wildlife.",
                    LongDescription = "Seychelles is an archipelago of 115 islands in the Indian Ocean, offering some of the world's most beautiful beaches with unique granite boulders. Anse Source d'Argent on La Digue Island is frequently voted the world's best beach. The islands are home to rare endemic species like giant tortoises and coco de mer palms. Luxury resorts, excellent diving, and a laid-back Creole culture make Seychelles a premium destination.",
                    CountryCode = "SYC",
                    DestinationTypeId = relaxId,
                    ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5",
                    TotalBookings = 876,
                    AverageRating = 4.94m,
                    ReviewCount = 678,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-149),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Edinburgh",
                    Description = "Scottish capital with medieval Old Town and stunning castle.",
                    LongDescription = "Edinburgh combines historic grandeur with modern vitality. Edinburgh Castle dominates the skyline, while the Royal Mile connects it to Holyrood Palace through the atmospheric Old Town. Climb Arthur's Seat for panoramic views, explore the Georgian New Town, visit the National Museum, and experience the world's largest arts festival each August. Scottish pubs offer whisky tastings and traditional haggis, while ghost tours reveal the city's dark history.",
                    CountryCode = "GBR",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1580993110624-28855435f848?w=800&h=600&fit=crop&auto=format&q=80",
                    TotalBookings = 1456,
                    AverageRating = 4.77m,
                    ReviewCount = 1189,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-96),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Queenstown",
                    Description = "New Zealand's adventure capital with stunning alpine scenery.",
                    LongDescription = "Queenstown is the adventure capital of the world, set on the shores of Lake Wakatipu with the dramatic Remarkables mountain range as backdrop. Try bungy jumping (invented here), jet boating, skydiving, paragliding, or wine tasting in nearby Gibbston Valley. In winter, world-class skiing awaits at Coronet Peak and The Remarkables. The stunning scenery includes Milford Sound, one of the world's most beautiful fjords.",
                    CountryCode = "NZL",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1507699622108-4be3abd695ad",
                    TotalBookings = 1234,
                    AverageRating = 4.86m,
                    ReviewCount = 987,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-111),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Lisbon",
                    Description = "Portugal's hilly coastal capital with pastel buildings and historic trams.",
                    LongDescription = "Lisbon spreads across seven hills overlooking the Tagus River, offering stunning viewpoints (miradouros) at every turn. Ride the iconic yellow tram 28 through narrow streets, visit the historic Belém Tower and Jerónimos Monastery, explore the bohemian Bairro Alto, and taste pastéis de nata (custard tarts). The city combines maritime heritage, Moorish influence, and contemporary culture with a relaxed Mediterranean vibe.",
                    CountryCode = "PRT",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1585208798174-6cedd86e019a",
                    TotalBookings = 1789,
                    AverageRating = 4.73m,
                    ReviewCount = 1456,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-69),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Galápagos Islands",
                    Description = "Unique ecosystem that inspired Darwin's theory of evolution.",
                    LongDescription = "The Galápagos Islands are a living laboratory of evolution, featuring species found nowhere else on Earth. Swim with sea lions, observe giant tortoises, watch blue-footed boobies, and snorkel with marine iguanas. Each island offers unique wildlife and landscapes, from volcanic formations to pristine beaches. Strict conservation rules protect this UNESCO World Heritage site, and guided tours provide insights into the extraordinary biodiversity that fascinated Charles Darwin.",
                    CountryCode = "ECU",
                    DestinationTypeId = adventureId,
                    ImageUrl = "https://images.unsplash.com/photo-1533587851505-d119e13fa0d7",
                    TotalBookings = 756,
                    AverageRating = 4.96m,
                    ReviewCount = 634,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-162),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Dubrovnik",
                    Description = "Croatia's pearl of the Adriatic with medieval walls and Game of Thrones fame.",
                    LongDescription = "Dubrovnik is a stunning walled city on the Adriatic coast, known as the 'Pearl of the Adriatic.' Walk atop the complete medieval city walls offering spectacular sea views, explore marble streets and baroque buildings in the Old Town (a Game of Thrones filming location), take a cable car to Mount Srđ for panoramic vistas, and island-hop to nearby Lokrum or the Elaphiti Islands. The city combines rich history with Mediterranean beauty.",
                    CountryCode = "HRV",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1555990538-c3c9eb0d5564?w=800&h=600&fit=crop&auto=format&q=80",
                    TotalBookings = 1567,
                    AverageRating = 4.81m,
                    ReviewCount = 1278,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-84),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Tuscany",
                    Description = "Italian region famous for rolling hills, vineyards, and Renaissance art.",
                    LongDescription = "Tuscany is the heart of Italian culture, cuisine, and Renaissance art. Explore Florence's art treasures, climb Pisa's Leaning Tower, wander medieval Siena, and drive through the scenic Chianti wine region with cypress-lined roads and hilltop villages. Stay in agriturismos (farmhouses), taste world-class wines and olive oil, enjoy truffle dishes, and soak in thermal springs. Tuscany perfectly blends natural beauty, history, and culinary excellence.",
                    CountryCode = "ITA",
                    DestinationTypeId = relaxId,
                    ImageUrl = "https://images.unsplash.com/photo-1523906834658-6e24ef2386f9",
                    TotalBookings = 1876,
                    AverageRating = 4.88m,
                    ReviewCount = 1534,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-76),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Cinque Terre",
                    Description = "Five colorful villages perched on Italy's Ligurian cliffs.",
                    LongDescription = "Cinque Terre consists of five pastel-colored fishing villages - Monterosso, Vernazza, Corniglia, Manarola, and Riomaggiore - clinging to steep coastal cliffs along the Italian Riviera. Connected by scenic hiking trails and trains, these UNESCO-protected villages offer stunning Mediterranean views, fresh seafood, pesto (originated here), and local wine. Car-free streets, terraced vineyards, and a slower pace of life create an authentic Italian coastal experience.",
                    CountryCode = "ITA",
                    DestinationTypeId = culturalId,
                    ImageUrl = "https://images.unsplash.com/photo-1516483638261-f4dbaf036963",
                    TotalBookings = 1345,
                    AverageRating = 4.85m,
                    ReviewCount = 1123,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-93),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Yosemite National Park",
                    Description = "California's granite wonderland with giant sequoias and waterfalls.",
                    LongDescription = "Yosemite National Park showcases nature's grandeur with towering granite cliffs like El Capitan and Half Dome, spectacular waterfalls including Yosemite Falls (North America's tallest), and ancient giant sequoia groves. Rock climbers from around the world tackle these legendary walls, while hikers explore over 750 miles of trails. Summer meadows bloom with wildflowers, and winter transforms the valley into a snowy wonderland. The park inspired conservation movements and continues to awe visitors.",
                    CountryCode = "USA",
                    DestinationTypeId = mountainId,
                    ImageUrl = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4",
                    TotalBookings = 2012,
                    AverageRating = 4.87m,
                    ReviewCount = 1678,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-108),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Buenos Aires",
                    Description = "Argentina's passionate capital known for tango, steaks, and European architecture.",
                    LongDescription = "Buenos Aires, the 'Paris of South America,' blends European elegance with Latin passion. Watch tango dancers in La Boca's colorful Caminito street, explore the grand avenues of Recoleta, visit the famous cemetery, enjoy world-class steaks and Malbec wine, and experience the vibrant nightlife. The city's passion for football (soccer), literature (home of Borges), and arts creates a rich cultural tapestry. Sunday markets and milongas (tango halls) reveal the city's soul.",
                    CountryCode = "ARG",
                    DestinationTypeId = cityId,
                    ImageUrl = "https://images.unsplash.com/photo-1589909202802-8f4aadce1849",
                    TotalBookings = 1456,
                    AverageRating = 4.71m,
                    ReviewCount = 1234,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-71),
                    LastModif = DateTime.UtcNow
                },
                new()
                {
                    Name = "Mount Fuji",
                    Description = "Japan's iconic snow-capped volcano and spiritual symbol.",
                    LongDescription = "Mount Fuji is Japan's highest mountain and most recognizable symbol, rising 3,776 meters above sea level. This perfectly conical active volcano is considered sacred and has inspired artists and poets for centuries. Visitors can climb to the summit during the July-August season, view it from the Five Lakes region, ride the Mt. Fuji Panoramic Ropeway, or enjoy distant views from Tokyo on clear days. Cherry blossoms at the base in spring create iconic photographs.",
                    CountryCode = "JPN",
                    DestinationTypeId = mountainId,
                    ImageUrl = "https://images.unsplash.com/photo-1490806843957-31f4c9a91c65",
                    TotalBookings = 1678,
                    AverageRating = 4.83m,
                    ReviewCount = 1389,
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow.AddDays(-119),
                    LastModif = DateTime.UtcNow
                }
            };
            
            _context.Destinations.AddRange(destinations);
        }
    }
}
