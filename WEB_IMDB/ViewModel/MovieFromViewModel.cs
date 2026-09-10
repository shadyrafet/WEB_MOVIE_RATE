namespace WEB_IMDB.ViewModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class MovieFromViewModel
{
    public int Id{get;set;}
    [Required,StringLength(250)]
    public string Title{get;set;}
    public int Year{get;set;}
    [Range(1,10)]
    public double? Rate{get;set;}
    [Required,MaxLength(250)]
    public string StoryLine{get;set;}
    [Display(Name = "Select Image")]
    public IFormFile Image{get;set;}
    [Display(Name = "Genre")]
    public int? GenreID{get;set;}
    public IEnumerable<Genre>? Genres{get;set;}
}