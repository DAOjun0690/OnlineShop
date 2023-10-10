using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using OnlineShop.Data;
using OnlineShop.Core.Models;
using ImageMagick;

namespace OnlineShop.Controllers;

public class PictureController : Controller
{
    private readonly OnlineShopContext _context;
    private readonly IConfiguration _configuration;

    private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
        {
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"}
        };

    private string ServerDestinationPath { get; set; }

    public PictureController(OnlineShopContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private void InitProperties(int Number)
    {
        string productId = Number.ToString();
#if DEBUG
        string uploadFolder = _configuration["UploadFolder"];
#elif RELEASE
        string  uploadFolder = "\\mounts\\" + _configuration["UploadFolder"];
#endif
        ServerDestinationPath = Path.Join(uploadFolder, productId);
        if (!Directory.Exists(ServerDestinationPath))
        {
            //新增資料夾
            Directory.CreateDirectory(ServerDestinationPath);
        }
    }

    /// <summary>
    /// Bare bones upload
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UploadDetail(int productId, IFormFile[] files)
    {
        InitProperties(productId);
        List<ProductImage> images;
        int start = 1;

        if (productId == 0)
        {
            images = new List<ProductImage>();
        }
        else
        {
            images = _context.ProductImage
                .Where(x => x.ProductId == productId && x.Type == ProductImageType.Detail).ToList();
            start = images.Count > 0 ? images.Max(p => p.Sequence) + 1 : 1;
        }

        int seq = start;

        Dictionary<string, string> returnData = new Dictionary<string, string>();
        if (files != null && files.Any())
        {
            foreach (IFormFile file in files)
            {
                // Make sure file is within valid size
                if (file.Length > 0 && file.Length < 15000000)  // About 15MB
                {
                    Guid guid = Guid.NewGuid();

                    // 將資料寫到db
                    ProductImage image = new ProductImage
                    {
                        Guid = guid,
                        ProductId = productId,
                        Sequence = seq,
                        FileName = file.FileName,
                        Type = ProductImageType.Detail
                    };
                    _context.ProductImage.Add(image);

                    // 儲存原圖
                    string saveName = guid + Path.GetExtension(file.FileName).ToLower();
                    string originPath = Path.Combine(ServerDestinationPath, "origin", saveName);
                    DirectoryExists(originPath);
                    using (FileStream stream = new FileStream(originPath, FileMode.Create))
                    {
                        // 程式寫入的本地資料夾裡面
                        await file.CopyToAsync(stream);

                        // Create two new image sizes
                        string thumbPath = Path.Combine(ServerDestinationPath, "thumb", saveName);
                        DirectoryExists(thumbPath);
                        ResizeCompressImage(stream, thumbPath, 120, 100);

                        string mediumPath = Path.Combine(ServerDestinationPath, "medium", saveName);
                        DirectoryExists(mediumPath);
                        ResizeCompressImage(stream, mediumPath, 1200, 80);
                    }

                    seq += 1;
                    returnData.Add(file.FileName, guid.ToString());
                }
                else
                {
                    // ignore the file. If you were a good programmer, you would tell the user why.
                    continue;
                }
            }
            await _context.SaveChangesAsync();
        }

        return Json(new { data = returnData });
    }

    /// <summary>
    /// CKEditor 使用的 圖片上傳
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="upload"></param>
    /// <returns></returns>
    public async Task<IActionResult> UploadContent(int productId, IFormFile upload)
    {
        if (upload.Length <= 0) return null!;

        Guid guid = Guid.NewGuid();
        var fileName = guid + Path.GetExtension(upload.FileName).ToLower();

        InitProperties(productId);

        var filePath = Path.Combine(ServerDestinationPath, "Content", fileName);
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            //新增資料夾
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            //程式寫入的本地資料夾裡面
            await upload.CopyToAsync(stream);
        }

        var images =
            _context.ProductImage.Where(x => x.ProductId == productId && x.Type == ProductImageType.Content).ToList();
        var start = images.Count > 0 ? images.Max(p => p.Sequence) + 1 : 1;

        // 將資料寫到db
        ProductImage image = new ProductImage
        {
            Guid = guid,
            ProductId = productId,
            Sequence = start,
            FileName = upload.FileName,
            Type = ProductImageType.Content
        };
        _context.ProductImage.Add(image);
        await _context.SaveChangesAsync();

        // 前端取值要使用的url 
        var url = $"{"../../Picture/Download?ProductId="}{productId}{"&Guid="}{guid}";

        return Json(new
        {
            uploaded = 1,
            fileName = fileName,
            url = url,
            error = new
            {
                message = "sucess"
            }
        });
    }

    /// <summary>
    /// 圖檔下載
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="guid"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public async Task<IActionResult> Download(int productId, Guid guid, string tag = "")
    {
        InitProperties(productId);

        // 根據guid，取得檔案類型，在設定路徑
        var image = _context.ProductImage.Where(x => x.Guid == guid).FirstOrDefault();
        if (image == null)
        {
            return NotFound();
        }

        string filePath = Path.Combine(ServerDestinationPath, image.Guid + Path.GetExtension(image.FileName).ToLower());
        // 如果是文章圖片
        if (image.Type == ProductImageType.Content)
        {
            filePath = Path.Combine(ServerDestinationPath, "Content", image.Guid + Path.GetExtension(image.FileName).ToLower());
        }
        else if (image.Type == ProductImageType.Detail)
        {
            // 目前開兩種檔案大小
            string sizeDir = tag == "M" ? "medium" : "thumb";
            filePath = Path.Combine(ServerDestinationPath, sizeDir, image.Guid + Path.GetExtension(image.FileName).ToLower());
        }

        var memoryStream = new MemoryStream();
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memoryStream);
        }
        memoryStream.Seek(0, SeekOrigin.Begin);

        // 回傳檔案到 Client 需要附上 Content Type，否則瀏覽器會解析失敗。
        return new FileStreamResult(memoryStream,
                                    _contentTypes[Path.GetExtension(filePath).ToLowerInvariant()])
        { FileDownloadName = Path.GetFileName(filePath) };
    }

    /// <summary>
    /// Delete an image from the database. It is not deleted from the hard drive.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="imageID"></param>
    /// <returns></returns>
    public async Task<IActionResult> Delete(Guid guid)
    {
        var image = _context.ProductImage.Where(x => x.Guid == guid).FirstOrDefault();
        if (image == null)
        {
            return NotFound();
        }

        // 刪除實體檔案

        // 刪除db資料
        _context.ProductImage.Remove(image);

        await _context.SaveChangesAsync();

        return Json(true);
    }

    /// <summary>
    /// 檢查路徑的資料夾是否存在
    /// 不在就新建一個
    /// </summary>
    /// <param name="filePath"></param>
    private void DirectoryExists(string filePath)
    {
        string dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            // 新增資料夾
            Directory.CreateDirectory(dirPath);
        }
    }

    /// <summary>
    /// 裁切圖片大小和壓縮圖片品質
    /// </summary>
    /// <param name="imgStream"></param>
    /// <param name="filePath"></param>
    /// <param name="imgWidth"></param>
    /// <param name="quality"></param>
    private void ResizeCompressImage(FileStream imgStream, string filePath, int? imgWidth, int quality)
    {
        try
        {
            imgStream.Seek(0, SeekOrigin.Begin);
            using (var image = new MagickImage(imgStream))
            {
                if (imgWidth.HasValue && imgWidth != 0)
                {
                    // Calculate the new height of the image given its desired width
                    int height = (int)Math.Round(image.Height * (imgWidth.Value / (float)image.Width));

                    // Resize the image using ImageMagick
                    image.Resize(imgWidth.Value, height);
                }
                // Set the quality level
                image.Quality = quality;

                // Save the image with the specified quality
                image.Write(filePath);
            }
        }
        catch (MagickException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}