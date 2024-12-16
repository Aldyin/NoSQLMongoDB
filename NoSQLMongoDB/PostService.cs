using MongoDB.Driver;
using System.Collections.Generic;
using System;

public class PostService
{
    private readonly IMongoCollection<Post> _posts;

    public PostService(IMongoDatabase database)
    {
        _posts = database.GetCollection<Post>("posts");
    }

    public void CreatePost(User user, string content)
    {
        var post = new Post
        {
            User = user,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        _posts.InsertOne(post);
    }

    public List<Post> GetFeed()
    {
        return _posts.Find(_ => true).SortByDescending(p => p.CreatedAt).ToList();
    }

    public List<Post> GetPostsByUser(User user)
    {
        return _posts.Find(p => p.User.Id == user.Id).ToList();
    }
}

