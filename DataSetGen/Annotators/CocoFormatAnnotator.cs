using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataSetGen.Annotators
{
    internal class CocoFormatAnnotator
    {
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
    public List<List<double>> Segmentation { get; set; }

    [JsonPropertyName("image_id")]
    public int ImageId { get; set; }

    [JsonPropertyName("area")]
    public double Area { get; set; }

    [JsonPropertyName("bbox")]
    public List<double> Bbox { get; set; }
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

public class Root
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

