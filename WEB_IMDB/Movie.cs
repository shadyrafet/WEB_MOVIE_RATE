using System.ComponentModel.DataAnnotations;

namespace WEB_IMDB;

public class Movie
{
    public int Id{get;set;}
    [Required,MaxLength(250)]
    public string Title{get;set;}
    public int Year{get;set;}
    public double Rate{get;set;}
    [Required,MaxLength(250)]
    public string StoryLine{get;set;}
    [Required]
    public byte[] Image{get;set;}
    public int GenreID{get;set;}
    public Genre  Genre{get;set;}
}