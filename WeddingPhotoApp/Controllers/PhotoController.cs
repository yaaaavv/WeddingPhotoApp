using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPhotoApp.Models;


public class PhotoController : Controller
{
    // DI ���ꂽ DB �R���e�L�X�g
    private readonly ApplicationDbContext _context;
    /// <summary>
    ///  �R���X�g���N�^�iDI�j
    /// </summary>
    public PhotoController(ApplicationDbContext context) => _context = context;

    /// <summary>
    /// 1 ���̎ʐ^�ƁA���̎ʐ^�ɕt�����R�����g��\������ڍ׃y�[�W�B
    /// ��: /photo/5
    /// </summary>
    [HttpGet("/photo/{id}")]
    public IActionResult Detail(int id)
    {
        // �ʐ^ + �R�����g�� 1 ���擾 (Include �� Comments �𓯎��ǂݍ���)
        var photo = _context.Photos.Include(p => p.Comments).FirstOrDefault(p => p.PhotoId == id);
        // id �������Ȃ� 404
        if (photo == null) return NotFound();
        // Views/Photo/Detail.cshtml
        return View(photo);
    }

    /// <summary>
    /// �ʏ�̃t�H�[�� POST �ŃR�����g��ǉ� �� �ڍ׃y�[�W�Ƀ��_�C���N�g�B
    /// </summary>
    [HttpPost("/photo/comment")]
    public IActionResult AddComment(int photoId, string userName, string text)
    {
        // ���̓`�F�b�N
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(text))
            return BadRequest("Name and comment are required");

        // �R�����g�G���e�B�e�B�𐶐����ĕۑ�
        var comment = new Comment
        {
            PhotoId = photoId,
            UserName = userName,
            Text = text,
            PostDate = DateTime.Now
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();
        // ������͍ēx�ڍ׃y�[�W�փ��_�C���N�g
        return RedirectToAction("Detail", new { id = photoId });
    }

    /// <summary>
    /// ���[�_������� Ajax POST ���󂯎��A�R�����g�� JSON �o�R�œo�^����B
    /// </summary>
    [HttpPost("/photo/comment/ajax")]
    public IActionResult AddCommentAjax([FromBody] CommentDto dto)
    {
        //���̓`�F�b�N
        if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Text))
            return BadRequest("���O�ƃR�����g�͕K�{�ł�");

        //DB�ɓo�^
        var comment = new Comment
        {
            PhotoId = dto.PhotoId,
            UserName = dto.UserName,
            Text = dto.Text,
            PostDate = DateTime.Now
        };
        _context.Comments.Add(comment);
        _context.SaveChanges();
        // 200 OK ��Ԃ������i��ʑJ�ڂȂ��j
        return Ok();
    }

    /// <summary>
    /// �R�����g�� 1 ���ȏ�t���Ă���ʐ^�������ꗗ�\���B
    /// page �p�����[�^���󂯎��A�߂�{�^���p�� ViewBag �ɕێ��B
    /// </summary>
    //�R�����g�t���ʐ^�ꗗ���
    [HttpGet("/photo/comments")]
    public IActionResult Comments(int page = 1)
    {
        var photosWithComments = _context.Photos
            .Include(p => p.Comments)
            .Where(p => p.Comments.Any())// �R�����g���t���Ă���ʐ^�̂�
            .ToList();

        if (photosWithComments == null) return NotFound();

       ViewBag.CurrentPage = page;// ���y�[�W�ԍ���߂�{�^���Ɏg��
        return View("~/Views/Home/Comments.cshtml", photosWithComments);
    }

    /// <summary>
    /// �w�肵���ʐ^ ID �̃R�����g�� LINE ���ŕ\���B
    /// </summary>
    //LINE���R�����g�ꗗ���
    [HttpGet("/photo/comment/{photoId}")]
    public IActionResult CommentList(int photoId)
    {
        var photo = _context.Photos
            .Include(p => p.Comments)
            .FirstOrDefault(p => p.PhotoId == photoId);

        if (photo == null) return NotFound();
        // �ꗗ�y�[�W�̃y�[�W�ԍ����N�G�������񂩂�擾
        ViewBag.Page = Request.Query["page"];

        return View(photo); // Views/Photo/CommentList.cshtml �ɑΉ�
    }

}