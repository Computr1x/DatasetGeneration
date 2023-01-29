using System.Drawing;
using System.Text.Json.Serialization;

namespace DataSetGen.Utils;

internal static class CocoFormatter
{
    internal static Annotation Annotate(int id, int categoryId, Point[][] contours)
    {
        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        List<List<float>> segmentation = new();
        for (int i = 0; i < contours.Length; i++)
        {
            List<float> segment = new();
            for (int j = 0; j < contours[i].Length; j++)
            {
                ref Point curPoint = ref contours[i][j];

                if (curPoint.X < minX) minX = curPoint.X;
                else if (curPoint.X > maxX) maxX = curPoint.X;
                if (curPoint.Y < minY) minY = curPoint.Y;
                else if (curPoint.Y > maxY) maxY = curPoint.Y;

                segment.Add(curPoint.X);
                segment.Add(curPoint.Y);
            }
            segmentation.Add(segment);
        }

        int width = maxX - minX;
        int height = maxY - minY;
        List<float> bbox = new() { minX, minY, width, height };
        int area = width * height;

        return new Annotation()
        {
            Id = id,
            CategoryId = categoryId,
            Iscrowd = 0,
            Segmentation = segmentation,
            ImageId = id,
            Area = area,
            Bbox = bbox,
        };
    }
}

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Annotation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("iscrowd")]
    public int Iscrowd { get; set; }

    [JsonPropertyName("segmentation")]
    public List<List<float>> Segmentation { get; set; }

    [JsonPropertyName("image_id")]
    public int ImageId { get; set; }

    [JsonPropertyName("area")]
    public float Area { get; set; }

    [JsonPropertyName("bbox")]
    public List<float> Bbox { get; set; }
}

public class Category
{
    [JsonPropertyName("supercategory")]
    public string Supercategory { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Image
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("license")]
    public int License { get; set; }

    [JsonPropertyName("coco_url")]
    public string CocoUrl { get; set; }

    [JsonPropertyName("flickr_url")]
    public string FlickrUrl { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    [JsonPropertyName("date_captured")]
    public string DateCaptured { get; set; }
}

public class Info
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("contributor")]
    public string Contributor { get; set; }

    [JsonPropertyName("date_created")]
    public string DateCreated { get; set; }
}

public class License
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class CocoData
{
    [JsonPropertyName("info")]
    public Info Info { get; set; }

    [JsonPropertyName("licenses")]
    public List<License> Licenses { get; set; }

    [JsonPropertyName("images")]
    public List<Image> Images { get; set; }

    [JsonPropertyName("annotations")]
    public List<Annotation> Annotations { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; }
}

