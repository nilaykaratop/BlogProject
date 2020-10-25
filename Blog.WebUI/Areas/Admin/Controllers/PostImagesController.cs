using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Business.Data;
using Blog.Entities.Models;
using Blog.Business.Repositories;
using Blog.Business.Services;
using Blog.WebUI.Utility;
using Microsoft.AspNetCore.Http;

namespace Blog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostImagesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IPostRepository _postRepository;
        private readonly IPostImageRepository _postImageRepository;
        private readonly IFileUpload _fileUpload;

        public PostImagesController(ApplicationContext context, IPostRepository postRepository, IPostImageRepository postImageRepository, IFileUpload fileUpload)
        {
            this._postRepository = postRepository;
            this._postImageRepository = postImageRepository;
            this._fileUpload = fileUpload;
            _context = context;
        }


        // GET: Admin/PostImages
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upload(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View(id);
        }
        [HttpPost]
        public IActionResult Upload(IFormFile[] file, Guid? id)
        {
            if (id.HasValue)
            {
                Post post = _postRepository.GetById(id.Value);
                if (post == null)
                {
                    return NotFound();
                }
                if (file.Length > 0)
                {
                    foreach (var item in file)
                    {
                        var result = _fileUpload.Upload(item);
                        if (result.FileResult == Utility.FileResult.Succeded)
                        {
                            PostImage image = new PostImage
                            {
                                ImageURL = result.FileUrl,
                                PostId = id.Value
                            };
                            _postImageRepository.Add(image);
                            _postImageRepository.Save();
                                
                        }
                    }
                }
            }
            return View();
        }
        // GET: Admin/PostImages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postImage = await _context.PostImages
                .Include(p => p.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postImage == null)
            {
                return NotFound();
            }

            return View(postImage);
        }

        // GET: Admin/PostImages/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content");
            return View();
        }

        // POST: Admin/PostImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageURL,Base64,Active,PostId,Id,CreatedComputerName,CreatedDate,CreatedBy,CreatedIp")] PostImage postImage)
        {
            if (ModelState.IsValid)
            {
                postImage.Id = Guid.NewGuid();
                _context.Add(postImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", postImage.PostId);
            return View(postImage);
        }

        // GET: Admin/PostImages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postImage = await _context.PostImages.FindAsync(id);
            if (postImage == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", postImage.PostId);
            return View(postImage);
        }

        // POST: Admin/PostImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ImageURL,Base64,Active,PostId,Id,CreatedComputerName,CreatedDate,CreatedBy,CreatedIp")] PostImage postImage)
        {
            if (id != postImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostImageExists(postImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Content", postImage.PostId);
            return View(postImage);
        }

        // GET: Admin/PostImages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postImage = await _context.PostImages
                .Include(p => p.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postImage == null)
            {
                return NotFound();
            }

            return View(postImage);
        }

        // POST: Admin/PostImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var postImage = await _context.PostImages.FindAsync(id);
            _context.PostImages.Remove(postImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostImageExists(Guid id)
        {
            return _context.PostImages.Any(e => e.Id == id);
        }


    }
}
