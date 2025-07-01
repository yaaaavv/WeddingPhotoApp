using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPhotoApp.Models;


public class PhotoController : Controller
{
    // DI された DB コンテキスト
    private readonly ApplicationDbContext _context;
    /// <summary>
    ///  コンストラクタ（DI）
    /// </summary>
    public PhotoController(ApplicationDbContext context) => _context = context;

    /// <summary>
    /// 1 枚の写真と、その写真に付いたコメントを表示する詳細ページ。
    /// 例: /photo/5
    /// </summary>
    [HttpGet("/photo/{id}")]
    public IActionResult Detail(int id)
    {
        // 写真 + コメントを 1 件取得 (Include で Comments を同時読み込み)
        var photo = _context.Photos.Include(p => p.Comments).FirstOrDefault(p => p.PhotoId == id);
        // id が無効なら 404
        if (photo == null) return NotFound();
        // Views/Photo/Detail.cshtml
        return View(photo);
    }

    /// <summary>
    /// 通常のフォーム POST でコメントを追加 → 詳細ページにリダイレクト。
    /// </summary>
    [HttpPost("/photo/comment")]
    public IActionResult AddComment(int photoId, string userName, string text)
    {
        // 入力チェック
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(text))
            return BadRequest("Name and comment are required");

        // コメントエンティティを生成して保存
        var comment = new Comment
        {
            PhotoId = photoId,
            UserName = userName,
            Text = text,
            PostDate = DateTime.Now
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();
        // 完了後は再度詳細ページへリダイレクト
        return RedirectToAction("Detail", new { id = photoId });
    }

    /// <summary>
    /// モーダルからの Ajax POST を受け取り、コメントを JSON 経由で登録する。
    /// </summary>
    [HttpPost("/photo/comment/ajax")]
    public IActionResult AddCommentAjax([FromBody] CommentDto dto)
    {
        //入力チェック
        if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Text))
            return BadRequest("名前とコメントは必須です");

        //DBに登録
        var comment = new Comment
        {
            PhotoId = dto.PhotoId,
            UserName = dto.UserName,
            Text = dto.Text,
            PostDate = DateTime.Now
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();
        // 200 OK を返すだけ（画面遷移なし）
        return Ok();
    }

    /// <summary>
    /// コメントが 1 件以上付いている写真だけを一覧表示。
    /// page パラメータを受け取り、戻るボタン用に ViewBag に保持。
    /// </summary>
    //コメント付き写真一覧画面
    [HttpGet("/photo/comments")]
    public IActionResult Comments(int page = 1)
    {
        var photosWithComments = _context.Photos
            .Include(p => p.Comments)
            .Where(p => p.Comments.Any())// コメントが付いている写真のみ
            .ToList();

        if (photosWithComments == null) return NotFound();

       ViewBag.CurrentPage = page;// 元ページ番号を戻るボタンに使う
        return View("~/Views/Home/Comments.cshtml", photosWithComments);
    }

    /// <summary>
    /// 指定した写真 ID のコメントを LINE 風で表示。
    /// </summary>
    //LINE風コメント一覧画面
    [HttpGet("/photo/comment/{photoId}")]
    public IActionResult CommentList(int photoId)
    {
        var photo = _context.Photos
            .Include(p => p.Comments)
            .FirstOrDefault(p => p.PhotoId == photoId);

        if (photo == null) return NotFound();
        // 一覧ページのページ番号をクエリ文字列から取得
        ViewBag.Page = Request.Query["page"];

        return View(photo); // Views/Photo/CommentList.cshtml に対応
    }

}