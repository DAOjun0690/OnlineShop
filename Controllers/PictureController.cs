using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

public class PictureController : Controller
{
    private readonly OnlineShopContext _context;

    private string ServerDestinationPath { get; set; }

    public PictureController(OnlineShopContext context) { _context = context; }

    private void InitProperties(int Number)
    {
        // 偏移量 folder，每一百個商品 一個資料夾
        string leftmost = (Number / 100).ToString();
        string productId = Number.ToString();
        ServerDestinationPath = Path.Join("\\pics\\", leftmost, productId);
    }

    /// <summary>
    /// Bare bones upload
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult UploadFiles(int productId, IFormFile[] files)
    {
        // 判斷 productId 是否為 0，0的話丟到暫存，其他則丟到對應檔案的資料夾底下
        if (productId == 0)
        {
            if (files != null && files.Any())
            {
                foreach (IFormFile file in files)
                {
                    // Make sure file is within valid size
                    if (file.Length > 0 && file.Length < 4000000)  // About 4MB
                    {
                        // 寫到 暫存資料夾
                        //string filePath = Path.Combine(Path.GetTempPath(), ServerTempPath, file.FileName);
                        //using (FileStream stream = new FileStream(filePath, FileMode.Create)) 
                        //{
                        //    file.CopyTo(stream);
                        //}
                    }
                }
            }
        }
        else
        {

        }



        try
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
                images = _context.ProductImage.Where(x => x.ProductId == productId).ToList();
                start = images.Count > 0 ? images.Max(p => p.Sequence) + 1 : 1;
            }
            int seq = start;

            List<string> uploadedFiles = new List<string>();
            string inputFileName;
            string serverSavePath;
            if (ModelState.IsValid)
            {
                //foreach (IFormFile file in files)
                //{
                //    // Make sure file is within valid size
                //    if (file.Length > 0 && file.Length < 4000000)  // About 4MB
                //    {
                //        // Might add code here to check extension to make sure it is allowed.
                //        // Generate a name for the uploaded file. Don't keep original name
                //        inputFileName = string.Format("{0}-{1:D2}.jpg", productId, seq);
                //        // We don't know much about the uploaded file. Store it outside of our website
                //        serverSavePath = Path.Combine(Path.GetTempPath(), inputFileName);
                //        // Write the uploaded file to serverSavePath. We know it is within our
                //        // valid size range and has acceptable extension.
                //        using (FileStream stream = new FileStream(serverSavePath, FileMode.Create))
                //        {
                //            file.CopyTo(stream);
                //            // Make a list of all files that have been uploaded. The user can select more
                //            // than one file at a time.
                //            uploadedFiles.Add(serverSavePath);
                //        }
                //        // We have not made copies of the uploaded files yet, but we are going to update our model.
                //        // We can always not commit changes to db if the files don't copy.
                //        ProductImage newImage = new ProductImage();
                //        newImage.ProductId = productId;
                //        newImage.Sequence = seq;
                //        newImage.LinkToLargeImage = Path.Combine(ServerDestinationPath, inputFileName);
                //        //newImage.LinkToMediumImage = Path.Combine(ServerDestinationPath, "medium", inputFileName);
                //        //newImage.LinkToSmallImage = Path.Combine(ServerDestinationPath, "thumb", inputFileName);
                //        item.Photos.Add(newImage);
                //        seq++;
                //    }
                //    else
                //    {
                //        // ignore the file. If you were a good programmer, you would tell the user why.
                //        continue;
                //    }
                //}
                //// Resize the images. 
                //int numPhotos = ResizePhotos(uploadedFiles, item);
                //// All went well, save to the database
                //_repository.Complete();
                //// Delete the original uploaded files
                //foreach (string s in uploadedFiles)
                //    System.IO.File.Delete(s);
            }
        }
        catch (FormatException ex)
        {
            throw ex;
            //return Content(ex.Message);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return Json("");
    }

    public class ImageUploadResponse
    {
        public int Uploaded { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Msg { get; set; } = string.Empty;
    }
    public async Task<IActionResult> UploadContent(int productId, IFormFile upload)
    {
        if (upload.Length <= 0) return null!;

        var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

        var filePath = Path.Combine(
               Directory.GetCurrentDirectory(), "wwwroot/image",
               fileName);

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            //程式寫入的本地資料夾裡面
            await upload.CopyToAsync(stream);
        }

        var url = $"{"/image/"}{fileName}";

        var reslult = new ImageUploadResponse
        {
            Uploaded = 1,
            FileName = fileName,
            Url = url,
            Msg = "sucess",
        };

        return Json(new
        {
            uploaded = reslult.Uploaded,
            fileName = reslult.FileName,
            url = reslult.Url,
            error = new
            {
                message = reslult.Msg
            }
        });
    }
}
