namespace JapanTravelApp.Services;

public class Tour
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public double Rating { get; set; } = 4.5;
}

public interface ITourService
{
    Task<List<Tour>> GetMyToursAsync();
    Task<List<Tour>> GetRecommendedToursAsync();
    Task<Tour?> GetTourByIdAsync(string id);
    Task<bool> BookTourAsync(string tourId);
    Task<bool> CancelTourAsync(string tourId);
}

public class TourService : ITourService
{
    private readonly List<Tour> _allTours = new()
    {
        new Tour
        {
            Id = "1",
            Title = "Tokyo Highlights",
            Description = "Experience the best of Tokyo including Shibuya, Harajuku, and Senso-ji Temple",
            Location = "Tokyo",
            Price = 8500,
            Date = DateTime.Now.AddDays(1),
            ImageUrl = "https://example.com/tokyo.jpg",
            Latitude = 35.6762,
            Longitude = 139.6503,
            Category = "Cultural",
            Duration = "8 hours",
            Rating = 4.8
        },
        new Tour
        {
            Id = "2",
            Title = "Kyoto Cultural Tour",
            Description = "Discover traditional Japan in Kyoto's temples and gardens",
            Location = "Kyoto",
            Price = 7500,
            Date = DateTime.Now.AddDays(3),
            ImageUrl = "https://example.com/kyoto.jpg",
            Latitude = 35.0116,
            Longitude = 135.7681,
            Category = "Historical",
            Duration = "6 hours",
            Rating = 4.9
        },
        new Tour
        {
            Id = "3",
            Title = "Osaka Food Adventure",
            Description = "Taste the best street food and local cuisine in Osaka",
            Location = "Osaka",
            Price = 6000,
            Date = DateTime.Now.AddDays(5),
            ImageUrl = "https://example.com/osaka.jpg",
            Latitude = 34.6937,
            Longitude = 135.5023,
            Category = "Food",
            Duration = "4 hours",
            Rating = 4.7
        },
        new Tour
        {
            Id = "4",
            Title = "Mount Fuji Day Trip",
            Description = "Visit Japan's iconic mountain with scenic views",
            Location = "Mount Fuji",
            Price = 12000,
            Date = DateTime.Now.AddDays(7),
            ImageUrl = "https://example.com/fuji.jpg",
            Latitude = 35.3606,
            Longitude = 138.7274,
            Category = "Nature",
            Duration = "10 hours",
            Rating = 4.6
        }
    };

    private readonly List<string> _myTourIds = new() { "1", "3" };

    public async Task<List<Tour>> GetMyToursAsync()
    {
        await Task.Delay(500); // Simulate network call
        return _allTours.Where(t => _myTourIds.Contains(t.Id)).ToList();
    }

    public async Task<List<Tour>> GetRecommendedToursAsync()
    {
        await Task.Delay(500); // Simulate network call
        return _allTours.Where(t => !_myTourIds.Contains(t.Id)).ToList();
    }

    public async Task<Tour?> GetTourByIdAsync(string id)
    {
        await Task.Delay(300); // Simulate network call
        return _allTours.FirstOrDefault(t => t.Id == id);
    }

    public async Task<bool> BookTourAsync(string tourId)
    {
        await Task.Delay(1000); // Simulate network call

        if (!_myTourIds.Contains(tourId))
        {
            _myTourIds.Add(tourId);
            return true;
        }

        return false;
    }

    public async Task<bool> CancelTourAsync(string tourId)
    {
        await Task.Delay(500); // Simulate network call
        return _myTourIds.Remove(tourId);
    }
}