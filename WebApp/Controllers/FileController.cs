using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using network2.Models;
using TagLib;
using WebApp.Services;
using WebApp.ViewModels;
using System.Drawing;
using System.Net.Mime;

namespace WebApp.Controllers
{
    public class FileController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;
        private PostService _postService;
        private UserService _userService;
        private IWebHostEnvironment _appEnvironment;

        public FileController(ApplicationContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _db = context;
            _appEnvironment = iWebHostEnvironment;
            _postService = new PostService(_db);
            _userService = new UserService(_db);
        }

        [HttpPost]
        public void AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    uploadedFile.CopyToAsync(fileStream);
                }

                _db.SaveChanges();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/Photos/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                PhotoModel model = new PhotoModel {
                        OwnerId = _userService.GetUserByName(User.Identity.Name).Id,
                        Path = path,
                        Comments = new List<int>(),
                        LikeUsers = new List<int>(),
                        Time = DateTime.Now
                };
                _db.PhotoModels.Add(model);
                _db.SaveChanges();
                _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Photos.Add(model.Id);
                _db.SaveChanges();
            }
            return RedirectToAction("Photos", "User", new { Id = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id } );
        }

        [HttpGet]
        public List<SongModel> GetSongsByUserId(int id)
        {
            UserModel user = _db.UserModels.FirstOrDefault( um => um.Id == id);
            return _db.SongModels.Where(sm => user.Songs.Contains(sm.Id)).ToList();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddSong(IFormFile uploadedFile)
        {
            if (uploadedFile != null && FileIsMusical(uploadedFile.FileName))
            {

                string path = "/Files/Songs/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                TagLib.File songData = TagLib.File.Create(_appEnvironment.WebRootPath + path);
                Image songPic = null;
                string picturePath = null;
                if (songData.Tag.Pictures.Length >= 1)
                {
                    var bin = (byte[])(songData.Tag.Pictures[0].Data.Data);
                    songPic = Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(100, 100, null, IntPtr.Zero);
                    picturePath = "/Files/SongPics/" +
                                         uploadedFile.FileName.Substring(0, uploadedFile.FileName.Length - 4) + ".jpg";
                    songPic.Save(@Url.Content(_appEnvironment.WebRootPath + picturePath));
                }
                SongModel song = new SongModel
                {
                    Path = path,
                    Name = songData.Tag.Title,
                    Album = songData.Tag.Album,
                    Artist = songData.Tag.JoinedPerformers,
                    PicturePath = picturePath
                };

                _db.SongModels.Add(song);
                _db.SaveChanges();
                _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Songs
                    .Add(_db.SongModels.FirstOrDefault(sm => sm.Path == path).Id);
                _db.SaveChanges();
            }

            return RedirectToAction("Music", "User", new { Id = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id } );
        }

        [HttpGet]
        public Photo GetPhotoById(int id)
        {
            Photo photo = Mappers.BuildPhoto(_db.PhotoModels.FirstOrDefault(pm => pm.Id == id));
            photo.Owner = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == photo.OwnerId));
            return photo;
        }
        
        static private bool FileIsMusical(string name)
        {
            int length = name.Length;
            int pointIndex = length;
            for (pointIndex = length; pointIndex >= 1; pointIndex--)
            {
                if (name[pointIndex - 1] == '.') break;
            }

            string extension = name.Substring(pointIndex);
            if (extension.Equals("mp3") ||
                extension.Equals("ogg") ||
                extension.Equals("wav") ||
                extension.Equals("aiff") ||
                extension.Equals("aac") ||
                extension.Equals("flac") || 
                extension.Equals("m4a"))
                return true;
            return false;
        }
        
        
    }
}