using Instagramm.Context;
using Instagramm.Enum;
using Instagramm.Models;
using Instagramm.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Instagramm.Controllers;

public class Instagram : Controller
{
    
    private InstagrammContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public Instagram(InstagrammContext db, UserManager<User> userManager, IWebHostEnvironment hostingEnvironment)
    {
        _db = db;
        _userManager = userManager;
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpGet]
    [ResponseCache(Duration = 360000, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult>  MyPage()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        
        UserViewModel userViewModel = new UserViewModel
        {
            Id = currentUser.Id,
            AvatarFileName = currentUser.AvatarFileName,
            UserName = currentUser.UserName,
            Name = currentUser.Name,
            Description = currentUser.Description,
            Gender = currentUser.Gender
        };
        
        ViewBag.Followers = _db.Followers.Where(f => f.FollowedId == currentUser.Id).Include(f => f.FollowerUser).ToList();
        ViewBag.Followeds = _db.Followers.Where(f => f.FollowerId == currentUser.Id).Include(f => f.FollowedUser).ToList();
        
        var posts = _db.Posts
            .Where(p => p.UserId == currentUser.Id)
            .OrderByDescending(p=>p.Creation).ToList();
        List<PostViewModel> postViewModels = new List<PostViewModel>();
        foreach (var post in posts)
        {
            postViewModels.Add(new PostViewModel(post));
        }
        ViewBag.Posts = postViewModels;
        return View(userViewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult>  UserPage(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        if (currentUser.Id == id)
        {
            return RedirectToAction("MyPage", "Instagram");
        }
        var user = _db.Users.FirstOrDefault(u => u.Id == id);
        UserViewModel userViewModel = new UserViewModel
        {
            Id = user.Id,
            AvatarFileName = user.AvatarFileName,
            UserName = user.UserName,
            Name = user.Name,
            Description = user.Description,
            Gender = user.Gender
        };

        ViewBag.Followers = _db.Followers.Where(f => f.FollowedId == id)
            .Include(f => f.FollowerUser).ToList();
        ViewBag.Followeds = _db.Followers.Where(f => f.FollowerId == id)
            .Include(f => f.FollowedUser).ToList();
        
        var followerLink = _db.Followers
            .FirstOrDefault(f => f.FollowerId == currentUser.Id && f.FollowedId == id);
        if (followerLink is null)
        {
            ViewBag.Subscribe = "Подписаться";
        }
        
        var posts = _db.Posts
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p=>p.Creation).ToList();
        List<PostViewModel> postViewModels = new List<PostViewModel>();
        foreach (var post in posts)
        {
            postViewModels.Add(new PostViewModel(post));
        }
        ViewBag.Posts = postViewModels;
        var roles = await _userManager.GetRolesAsync(currentUser);
        ViewBag.Roles = roles;
        return View(userViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> AddPost(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        AddPostViewModel addPostViewModel = new AddPostViewModel
        {
            UserId = id
        };
        return View(addPostViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddPost(AddPostViewModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        if (ModelState.IsValid)
        {
            if (model.PostPictureFile != null && model.PostPictureFile.Length > 0)
            {
                string extension = Path.GetExtension(model.PostPictureFile.FileName);
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ViewBag.Error = "Расширение файла должно быть .jpg, .jpeg или .png";
                    return View();
                }
                
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "posts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PostPictureFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.PostPictureFile.CopyTo(stream);
                }

                model.PostFileName = uniqueFileName;
            }
            else
            {
                return RedirectToAction("MyPage");
            }
            
            Post post = new Post
            {
                Description = model.Description,
                LikesCount = 0,
                CommentsCount = 0,
                PostFileName = model.PostFileName,
                Creation = DateTime.Now,
                UserId = model.UserId
            };
            _db.Add(post);
            _db.SaveChanges();
            return RedirectToAction("MyPage");
        }

        ViewBag.Error = "Не удалось добавить новый пост";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Like(int postId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
    
        var post = _db.Posts.FirstOrDefault(p => p.Id == postId);
        var like = _db.Likes.FirstOrDefault(l => l.PostId == post.Id && l.UserId == currentUser.Id);
    
        if (like == null)
        {
            var newLike = new Like
            {
                PostId = post.Id,
                UserId = currentUser.Id,
                Creation = DateTime.Now
            };
            _db.Likes.Add(newLike);
            post.LikesCount += 1;
            await _db.SaveChangesAsync();
        }
        else
        {
            _db.Likes.Remove(like);
            post.LikesCount -= 1;
            await _db.SaveChangesAsync();
        }
        
        LikeViewModel likeViewModel = new LikeViewModel()
        {
            PostId = postId,
            LikesCount = post.LikesCount
        };
    
        if (postId > 0)
        {
            return Json(likeViewModel);
        }
       
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> AddComment(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        var post = _db.Posts.Include(p=>p.User).FirstOrDefault(p => p.Id == id);
        PostViewModel postViewModel = new PostViewModel
        {
            Id = id,
            UserId = post.UserId,
            UserName = post.User.UserName,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            AvatarFileName = post.User.AvatarFileName,
            PostFileName = post.PostFileName,
            DescriptionForPost = post.Description,
            Creation = post.Creation
        };
        
        var comments = _db.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == post.Id)
            .OrderByDescending(c => c.Creation) 
            .ToList();

        List<CommentViewModel> commentViewModels = new List<CommentViewModel>();
        foreach (var comment in comments)
        {
            CommentViewModel commentViewModel = new CommentViewModel();
            commentViewModel.Id = comment.Id;
            commentViewModel.AvatarFileName = comment.User.AvatarFileName;
            commentViewModel.UserName = comment.User.UserName;
            commentViewModel.Description = comment.Description;
            commentViewModel.Creation = comment.Creation;
            commentViewModel.UserId = comment.UserId;
            commentViewModel.CurrentUserId = currentUser.Id;
            commentViewModels.Add(commentViewModel);
        }
        ViewBag.Roles = await _userManager.GetRolesAsync(currentUser);
        ViewBag.Post = commentViewModels;
        return View(postViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(PostViewModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        if (model is null)
        {
            return RedirectToAction("MyPage");
        }

        var post = _db.Posts.FirstOrDefault(p => p.Id == model.Id);
        Comment comment = new Comment
        {
            Description = model.DescriptionForComment,
            Creation = DateTime.Now,
            UserId = currentUser.Id,
            PostId = model.Id
        };
        _db.Comments.Add(comment);
        post.CommentsCount += 1;
        _db.SaveChanges();
        return RedirectToAction("AddComment", "Instagram", new { id = post.Id});
    }

    [HttpGet]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        if (id == 0)
        {
            return BadRequest();
        }
        var comment = _db.Comments.FirstOrDefault(c => c.Id == id);
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            if (comment.UserId == currentUser.Id || role == "admin")
            {
                var post = _db.Posts.FirstOrDefault(p => p.Id == comment.PostId);
                    _db.Comments.Remove(comment);
                    post.CommentsCount -= 1;
                    _db.SaveChanges();
                    return RedirectToAction("AddComment", "Instagram", new {id = post.Id} );
            }
        }
        return RedirectToAction("MyPage");
    }
    
    [HttpGet]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        if (id == 0)
        {
            return BadRequest();
        }
        var post = _db.Posts.FirstOrDefault(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            if (post.UserId == currentUser.Id || role == "admin")
            {
                DeletePostViewModel deletePostViewModel = new DeletePostViewModel()
                {
                    PostId = post.Id,
                    PostFileName = post.PostFileName,
                    LikesCount = post.LikesCount,
                    CommentsCount = post.CommentsCount,
                    DescriptionForPost = post.Description
                };
                return Json(deletePostViewModel);
            }
        }
        TempData["NotDelete"] = "Пост может удалить только создатель данного поста";
        return RedirectToAction("MyPage");
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> ConfirmDeletePost(int? id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        var post = _db.Posts.FirstOrDefault(p => p.Id == id);
        string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "posts", post.PostFileName);
        
        if (id is null)
            return BadRequest();
        
        if (post is null)
            return NotFound();
        
        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }
        
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            if (post.UserId == currentUser.Id || role == "admin")
            {
                _db.Posts.Remove(post);
                _db.SaveChanges();
                return Json(post);
            }
        }
        return Forbid();
    }
    
    [HttpGet]
    public async Task<IActionResult> EditPost(int id)
    {
        
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        var post = _db.Posts.Include(p=>p.User).FirstOrDefault(p => p.Id == id);
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            if (currentUser.Id == post.UserId || role == "admin")
            {
                PostViewModel postViewModel = new PostViewModel
                {
                    Id = id,
                    UserName = post.User.UserName,
                    LikesCount = post.LikesCount,
                    CommentsCount = post.CommentsCount,
                    PostFileName = post.PostFileName,
                    DescriptionForPost = post.Description,
                };
                return Json(postViewModel);
            }
        }   
        
        return RedirectToAction("MyPage");
    }
    
    [HttpPost]
    public async Task<IActionResult> EditPost(int postId, string postDescription)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Register", "Account");
        }
        if (postId == 0)
        {
            return RedirectToAction("Index");
        }

        var post = _db.Posts.FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            return NotFound();
        }

        post.Description = postDescription;

        try
        {
            var roles = await _userManager.GetRolesAsync(currentUser);
            foreach (var role in roles)
                if (post.UserId == currentUser.Id || role == "admin")
                {
                    _db.Posts.Update(post);
                    _db.SaveChanges();
                    return RedirectToAction("EditPost", "Instagram", new { id = post.Id }); 
                }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }
        return Forbid();
    }

    [HttpGet]
    public async Task<IActionResult> EditUserInfo(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == id);

        UserViewModel userViewModel = new UserViewModel()
        {
            Id = user.Id,
            AvatarFileName = user.AvatarFileName,
            Name = user.Name,
            Description = user.Description,
        };
        return View(userViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditUserInfo(UserViewModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == model.Id);
        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            string extension = Path.GetExtension(model.AvatarFile.FileName);
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                ViewBag.Error = "Расширение файла должно быть .jpg, .jpeg или .png";
                return RedirectToAction("EditUserInfo");
            }
                
            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "avatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
        
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.AvatarFile.CopyTo(stream);
            }
        
            model.AvatarFileName = uniqueFileName;
        }
        else
        {
            model.AvatarFileName = user.AvatarFileName;
        }

        user.Name = model.Name;
        user.Description = model.Description;
        user.AvatarFileName = model.AvatarFileName;
        
        if (model.GenderInfo == "Male")
        {
            user.Gender = Gender.Man;
        }
        else if (model.GenderInfo == "Female")
        {
            user.Gender = Gender.Woman;
        }
        else if (model.GenderInfo == "Uncertainty")
        {
            user.Gender = Gender.Uncertainty;
        }
        else if (model.GenderInfo == "None")
        {
            user.Gender = Gender.None;
        }
        
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        if (currentUser.Id == model.Id || role == "admin")
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }
        return RedirectToAction("MyPage");
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        var posts = _db.Posts
            .Include(p => p.User)
            .OrderByDescending(p => p.Creation).ToList();
        
        List<PostViewModel> postViewModels = new List<PostViewModel>();
        foreach (var post in posts)
        {
            PostViewModel postViewModel = new PostViewModel();
            postViewModel.Id = post.Id;
            postViewModel.UserId = post.UserId;
            postViewModel.UserName = post.User.UserName;
            postViewModel.LikesCount = post.LikesCount;
            postViewModel.CommentsCount = post.CommentsCount;
            postViewModel.AvatarFileName = post.User.AvatarFileName;
            postViewModel.PostFileName = post.PostFileName;
            postViewModel.Creation = post.Creation;
            postViewModels.Add(postViewModel);
        }
        var roles = await _userManager.GetRolesAsync(currentUser);
        ViewBag.Roles = roles;
        return View(postViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> CheckSubscription(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
    
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("MyPage");
        }
        
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user is not null)
        {
            return RedirectToAction("MyPage");
        }
        
        var followerLink = _db.Followers
            .FirstOrDefault(f => f.FollowerId == currentUser.Id && f.FollowedId == userId);
        var Istrue = false;
        
        if (followerLink is null)
            Istrue = true;
        
        return Json(Istrue);
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("MyPage");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return RedirectToAction("MyPage");
        }

        var followerLink = _db.Followers
            .FirstOrDefault(f => f.FollowerId == currentUser.Id && f.FollowedId == userId);
        var Istrue = false;
        if (followerLink is null)
        {
            Follower follower = new Follower()
            {
                FollowedId = userId,
                FollowerId = currentUser.Id,
                Creation = DateTime.Now
            };

            _db.Followers.Add(follower);
            _db.SaveChanges();
            Istrue = true;
        }
        else
        {
            _db.Followers.Remove(followerLink);
            _db.SaveChanges();
        }
        var Followers = _db.Followers.Where(f => f.FollowedId == userId)
            .Include(f => f.FollowerUser).ToList();
        var Followeds = _db.Followers.Where(f => f.FollowerId == userId)
            .Include(f => f.FollowedUser).ToList();

        SubscribAndFollowers subscribAndFollowers = new SubscribAndFollowers()
        {
            Subscribe = Istrue,
            Follower = Followers.Count,
            Followed = Followeds.Count
        };
        return Json(subscribAndFollowers);
    }

    [HttpGet]
    public async Task<IActionResult> MyFollowedUsers()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        
        var followedUsers = _db.Followers
            .Where(f => f.FollowerId == currentUser.Id)
            .Select(f => f.FollowedId)
            .ToList();
        
        var postsFromFollowedUsers = _db.Posts
            .Where(p => followedUsers.Contains(p.UserId))
            .Include(p=>p.User)
            .OrderByDescending(p => p.Creation)
            .ToList();
        
        List<PostViewModel> postViewModels = new List<PostViewModel>();
        foreach (var post in postsFromFollowedUsers)
        {
            PostViewModel postViewModel = new PostViewModel();
            postViewModel.Id = post.Id;
            postViewModel.UserId = post.UserId;
            postViewModel.UserName = post.User.UserName;
            postViewModel.LikesCount = post.LikesCount;
            postViewModel.CommentsCount = post.CommentsCount;
            postViewModel.AvatarFileName = post.User.AvatarFileName;
            postViewModel.PostFileName = post.PostFileName;
            postViewModel.Creation = post.Creation;
            postViewModels.Add(postViewModel);
        }

        var roles = await _userManager.GetRolesAsync(currentUser);
        ViewBag.Roles = roles;
        return View(postViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> FindUserFromFollowed(string userInfo)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        IQueryable<Follower> followersQuery = _db.Followers
            .Where(f => f.FollowerId == currentUser.Id)
            .Include(f => f.FollowedUser);

        IQueryable<User> users = followersQuery.Select(f => f.FollowedUser);

        IQueryable<UserViewModel> model = users.Select(user => new UserViewModel
        {
            Id = user.Id,
            AvatarFileName = user.AvatarFileName,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name
        });
        
        if (!string.IsNullOrEmpty(userInfo))
        {
            model = model.Where(u => u.UserName.Contains(userInfo) || 
                                     u.Name.Contains(userInfo) || u.Email.Contains(userInfo));
        }
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> FindUser(string userInfo)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        IQueryable<User> users = _db.Users;
        IQueryable<UserViewModel> model = users.Select(user => new UserViewModel
        {
            Id = user.Id,
            AvatarFileName = user.AvatarFileName,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name
        });
        
        if (!string.IsNullOrEmpty(userInfo))
        {
            model = model.Where(u => u.UserName.Contains(userInfo) || 
                                     u.Name.Contains(userInfo) || u.Email.Contains(userInfo));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUserPage(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == id);
        var roles = await _userManager.GetRolesAsync(currentUser);
        UserViewModel userViewModel = new UserViewModel();
        foreach (var role in roles)
        {
            if (currentUser.Id == id || role == "admin")
            {
                userViewModel.Id = user.Id;
                userViewModel.UserName = user.UserName;
                userViewModel.Name = user.Name;
                userViewModel.Description = user.Description;
                userViewModel.AvatarFileName = user.AvatarFileName;

            }
        }
        return View(userViewModel);
    }
    
    [HttpPost]
    [ActionName("DeleteUserPage")]
    public async Task<IActionResult> ConfirmDeleteUser(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        if (string.IsNullOrEmpty(id))
            return BadRequest();
        var user = _db.Users.FirstOrDefault(u => u.Id == id);
        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            if (currentUser.Id == id)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return RedirectToAction("Register", "Account");
            }
            if (role == "admin")
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }
        return RedirectToAction("Index");
    }
}