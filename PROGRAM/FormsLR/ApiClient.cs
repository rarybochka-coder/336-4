using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public static class ApiClient
{
    private static readonly HttpClient client = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
    private static string BaseUrl = "http://localhost:5000/api";

    public static DataTable GetTable(string endpoint)
    {
        var response = client.GetStringAsync($"{BaseUrl}/{endpoint}").Result;
        var rows = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response);
        var dt = new DataTable();

        if (rows == null || rows.Count == 0) return dt;

        foreach (var key in rows[0].Keys)
            dt.Columns.Add(key);

        foreach (var row in rows)
        {
            var dr = dt.NewRow();
            foreach (var key in row.Keys)
                dr[key] = row[key].ToString();
            dt.Rows.Add(dr);
        }

        return dt;
    }

    public static DataTable Search(string endpoint, string query)
    {
        var response = client.GetStringAsync(
            $"{BaseUrl}/{endpoint}/search?q={Uri.EscapeDataString(query)}").Result;
        var rows = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response);
        var dt = new DataTable();

        if (rows == null || rows.Count == 0) return dt;

        foreach (var key in rows[0].Keys)
            dt.Columns.Add(key);

        foreach (var row in rows)
        {
            var dr = dt.NewRow();
            foreach (var key in row.Keys)
                dr[key] = row[key].ToString();
            dt.Rows.Add(dr);
        }

        return dt;
    }

    public static void Add(string endpoint, Dictionary<string, string> data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"{BaseUrl}/{endpoint}", content).Result;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"API error: {response.StatusCode}");
    }

    public static void Delete(string endpoint, int id)
    {
        var response = client.DeleteAsync($"{BaseUrl}/{endpoint}/{id}").Result;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"API error: {response.StatusCode}");
    }
}