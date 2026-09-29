from pathlib import Path
from datetime import date

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_CELL_VERTICAL_ALIGNMENT, WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Inches, Pt, RGBColor


OUT = Path(r"E:\Projeler\MeteorGaleri\docs\Canvasia-Sosyal-Medya-OAuth-Kurulum-Rehberi.docx")

NAVY = "1F3A5F"
BLUE = "2E74B5"
DARK_BLUE = "1F4D78"
PALE_BLUE = "E8EEF5"
LIGHT_GRAY = "F2F4F7"
MID_GRAY = "667085"
GOLD = "9A6B00"
RED = "9B1C1C"
GREEN = "247A52"
WHITE = "FFFFFF"
BLACK = "111827"


def set_cell_shading(cell, fill):
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = tc_pr.find(qn("w:shd"))
    if shd is None:
        shd = OxmlElement("w:shd")
        tc_pr.append(shd)
    shd.set(qn("w:fill"), fill)


def set_cell_margins(cell, top=100, start=120, bottom=100, end=120):
    tc = cell._tc
    tc_pr = tc.get_or_add_tcPr()
    tc_mar = tc_pr.first_child_found_in("w:tcMar")
    if tc_mar is None:
        tc_mar = OxmlElement("w:tcMar")
        tc_pr.append(tc_mar)
    for m, v in (("top", top), ("start", start), ("bottom", bottom), ("end", end)):
        node = tc_mar.find(qn(f"w:{m}"))
        if node is None:
            node = OxmlElement(f"w:{m}")
            tc_mar.append(node)
        node.set(qn("w:w"), str(v))
        node.set(qn("w:type"), "dxa")


def set_table_geometry(table, widths_dxa, indent_dxa=120):
    table.autofit = False
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    tbl_pr = table._tbl.tblPr
    tbl_w = tbl_pr.find(qn("w:tblW"))
    if tbl_w is None:
        tbl_w = OxmlElement("w:tblW")
        tbl_pr.append(tbl_w)
    tbl_w.set(qn("w:w"), str(sum(widths_dxa)))
    tbl_w.set(qn("w:type"), "dxa")

    tbl_ind = tbl_pr.find(qn("w:tblInd"))
    if tbl_ind is None:
        tbl_ind = OxmlElement("w:tblInd")
        tbl_pr.append(tbl_ind)
    tbl_ind.set(qn("w:w"), str(indent_dxa))
    tbl_ind.set(qn("w:type"), "dxa")

    grid = table._tbl.tblGrid
    for child in list(grid):
        grid.remove(child)
    for width in widths_dxa:
        col = OxmlElement("w:gridCol")
        col.set(qn("w:w"), str(width))
        grid.append(col)

    for row in table.rows:
        for idx, cell in enumerate(row.cells):
            tc_pr = cell._tc.get_or_add_tcPr()
            tc_w = tc_pr.find(qn("w:tcW"))
            if tc_w is None:
                tc_w = OxmlElement("w:tcW")
                tc_pr.append(tc_w)
            tc_w.set(qn("w:w"), str(widths_dxa[idx]))
            tc_w.set(qn("w:type"), "dxa")
            set_cell_margins(cell)
            cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER


def set_repeat_table_header(row):
    tr_pr = row._tr.get_or_add_trPr()
    tbl_header = OxmlElement("w:tblHeader")
    tbl_header.set(qn("w:val"), "true")
    tr_pr.append(tbl_header)


def set_run_font(run, size=11, color=BLACK, bold=False, italic=False):
    run.font.name = "Calibri"
    run._element.get_or_add_rPr().rFonts.set(qn("w:ascii"), "Calibri")
    run._element.get_or_add_rPr().rFonts.set(qn("w:hAnsi"), "Calibri")
    run.font.size = Pt(size)
    run.font.color.rgb = RGBColor.from_string(color)
    run.bold = bold
    run.italic = italic


def add_hyperlink(paragraph, text, url, color=BLUE):
    part = paragraph.part
    rid = part.relate_to(url, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", is_external=True)
    hyperlink = OxmlElement("w:hyperlink")
    hyperlink.set(qn("r:id"), rid)
    new_run = OxmlElement("w:r")
    r_pr = OxmlElement("w:rPr")
    r_fonts = OxmlElement("w:rFonts")
    r_fonts.set(qn("w:ascii"), "Calibri")
    r_fonts.set(qn("w:hAnsi"), "Calibri")
    r_pr.append(r_fonts)
    c = OxmlElement("w:color")
    c.set(qn("w:val"), color)
    r_pr.append(c)
    u = OxmlElement("w:u")
    u.set(qn("w:val"), "single")
    r_pr.append(u)
    new_run.append(r_pr)
    t = OxmlElement("w:t")
    t.text = text
    new_run.append(t)
    hyperlink.append(new_run)
    paragraph._p.append(hyperlink)


def add_page_number(paragraph):
    paragraph.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    run = paragraph.add_run("Sayfa ")
    set_run_font(run, size=9, color=MID_GRAY)
    fld = OxmlElement("w:fldSimple")
    fld.set(qn("w:instr"), "PAGE")
    paragraph._p.append(fld)


def add_num_definition(doc, num_id, abstract_id, kind="decimal"):
    numbering = doc.part.numbering_part.element
    abstract = OxmlElement("w:abstractNum")
    abstract.set(qn("w:abstractNumId"), str(abstract_id))
    multi = OxmlElement("w:multiLevelType")
    multi.set(qn("w:val"), "singleLevel")
    abstract.append(multi)
    lvl = OxmlElement("w:lvl")
    lvl.set(qn("w:ilvl"), "0")
    start = OxmlElement("w:start")
    start.set(qn("w:val"), "1")
    lvl.append(start)
    num_fmt = OxmlElement("w:numFmt")
    num_fmt.set(qn("w:val"), "bullet" if kind == "bullet" else "decimal")
    lvl.append(num_fmt)
    lvl_text = OxmlElement("w:lvlText")
    lvl_text.set(qn("w:val"), "â€¢" if kind == "bullet" else "%1.")
    lvl.append(lvl_text)
    suff = OxmlElement("w:suff")
    suff.set(qn("w:val"), "tab")
    lvl.append(suff)
    p_pr = OxmlElement("w:pPr")
    tabs = OxmlElement("w:tabs")
    tab = OxmlElement("w:tab")
    tab.set(qn("w:val"), "num")
    tab.set(qn("w:pos"), "540")
    tabs.append(tab)
    p_pr.append(tabs)
    ind = OxmlElement("w:ind")
    ind.set(qn("w:left"), "540")
    ind.set(qn("w:hanging"), "270")
    p_pr.append(ind)
    lvl.append(p_pr)
    r_pr = OxmlElement("w:rPr")
    fonts = OxmlElement("w:rFonts")
    fonts.set(qn("w:ascii"), "Calibri")
    fonts.set(qn("w:hAnsi"), "Calibri")
    r_pr.append(fonts)
    lvl.append(r_pr)
    abstract.append(lvl)
    numbering.append(abstract)

    num = OxmlElement("w:num")
    num.set(qn("w:numId"), str(num_id))
    abstract_ref = OxmlElement("w:abstractNumId")
    abstract_ref.set(qn("w:val"), str(abstract_id))
    num.append(abstract_ref)
    numbering.append(num)


def apply_numbering(paragraph, num_id):
    p_pr = paragraph._p.get_or_add_pPr()
    num_pr = OxmlElement("w:numPr")
    ilvl = OxmlElement("w:ilvl")
    ilvl.set(qn("w:val"), "0")
    num_id_node = OxmlElement("w:numId")
    num_id_node.set(qn("w:val"), str(num_id))
    num_pr.append(ilvl)
    num_pr.append(num_id_node)
    p_pr.append(num_pr)


def add_bullet(doc, text, bold_prefix=None):
    p = doc.add_paragraph()
    apply_numbering(p, 42)
    p.paragraph_format.space_after = Pt(4)
    p.paragraph_format.line_spacing = 1.25
    if bold_prefix and text.startswith(bold_prefix):
        r = p.add_run(bold_prefix)
        set_run_font(r, bold=True)
        r = p.add_run(text[len(bold_prefix):])
        set_run_font(r)
    else:
        r = p.add_run(text)
        set_run_font(r)
    return p


def add_step(doc, text):
    p = doc.add_paragraph()
    apply_numbering(p, 43)
    p.paragraph_format.space_after = Pt(4)
    p.paragraph_format.line_spacing = 1.25
    set_run_font(p.add_run(text))
    return p


def add_checkbox(doc, text):
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.18)
    p.paragraph_format.first_line_indent = Inches(-0.18)
    p.paragraph_format.space_after = Pt(4)
    p.paragraph_format.line_spacing = 1.2
    set_run_font(p.add_run("â˜ "), size=11, color=BLUE, bold=True)
    set_run_font(p.add_run(text))
    return p


def add_callout(doc, title, text, fill=PALE_BLUE, accent=BLUE):
    table = doc.add_table(rows=1, cols=1)
    set_table_geometry(table, [9360])
    cell = table.cell(0, 0)
    set_cell_shading(cell, fill)
    p = cell.paragraphs[0]
    p.paragraph_format.space_after = Pt(3)
    set_run_font(p.add_run(title + "\n"), size=11, color=accent, bold=True)
    set_run_font(p.add_run(text), size=10.5, color=BLACK)
    set_repeat_table_header(table.rows[0])
    doc.add_paragraph().paragraph_format.space_after = Pt(1)


def add_key_value_table(doc, rows, widths=(2700, 6660), header=None):
    data_rows = len(rows) + (1 if header else 0)
    table = doc.add_table(rows=data_rows, cols=2)
    set_table_geometry(table, list(widths))
    cursor = 0
    if header:
        for idx, value in enumerate(header):
            cell = table.rows[0].cells[idx]
            set_cell_shading(cell, PALE_BLUE)
            p = cell.paragraphs[0]
            set_run_font(p.add_run(value), size=10, color=NAVY, bold=True)
        set_repeat_table_header(table.rows[0])
        cursor = 1
    for r_idx, (label, value) in enumerate(rows, start=cursor):
        left, right = table.rows[r_idx].cells
        if (r_idx - cursor) % 2 == 1:
            set_cell_shading(left, "F8FAFC")
            set_cell_shading(right, "F8FAFC")
        lp = left.paragraphs[0]
        rp = right.paragraphs[0]
        set_run_font(lp.add_run(label), size=10, color=NAVY, bold=True)
        set_run_font(rp.add_run(value), size=10, color=BLACK)
    if not header and table.rows:
        set_repeat_table_header(table.rows[0])
    return table


def add_source(doc, title, url):
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.18)
    p.paragraph_format.first_line_indent = Inches(-0.18)
    p.paragraph_format.space_after = Pt(4)
    set_run_font(p.add_run("â€¢ "), color=MID_GRAY)
    add_hyperlink(p, title, url)


def add_page_break(doc):
    doc.add_page_break()


doc = Document()
section = doc.sections[0]
section.page_width = Inches(8.5)
section.page_height = Inches(11)
section.top_margin = Inches(0.8)
section.bottom_margin = Inches(0.75)
section.left_margin = Inches(1)
section.right_margin = Inches(1)
section.header_distance = Inches(0.45)
section.footer_distance = Inches(0.45)

styles = doc.styles
normal = styles["Normal"]
normal.font.name = "Calibri"
normal._element.rPr.rFonts.set(qn("w:ascii"), "Calibri")
normal._element.rPr.rFonts.set(qn("w:hAnsi"), "Calibri")
normal.font.size = Pt(11)
normal.font.color.rgb = RGBColor.from_string(BLACK)
normal.paragraph_format.space_after = Pt(6)
normal.paragraph_format.line_spacing = 1.25

for name, size, color, before, after in (
    ("Heading 1", 16, BLUE, 18, 10),
    ("Heading 2", 13, BLUE, 14, 7),
    ("Heading 3", 12, DARK_BLUE, 10, 5),
):
    style = styles[name]
    style.font.name = "Calibri"
    style._element.rPr.rFonts.set(qn("w:ascii"), "Calibri")
    style._element.rPr.rFonts.set(qn("w:hAnsi"), "Calibri")
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = RGBColor.from_string(color)
    style.paragraph_format.space_before = Pt(before)
    style.paragraph_format.space_after = Pt(after)
    style.paragraph_format.keep_with_next = True

add_num_definition(doc, 42, 42, "bullet")
add_num_definition(doc, 43, 43, "decimal")

header = section.header
hp = header.paragraphs[0]
hp.alignment = WD_ALIGN_PARAGRAPH.LEFT
set_run_font(hp.add_run("CANVASIA SOCIAL  |  PLATFORM YETKÄ°LENDÄ°RME REHBERÄ°"), size=8.5, color=MID_GRAY, bold=True)

footer = section.footer
fp = footer.paragraphs[0]
add_page_number(fp)

# Cover / customer pack opening
p = doc.add_paragraph()
p.paragraph_format.space_before = Pt(18)
p.paragraph_format.space_after = Pt(2)
set_run_font(p.add_run("PROJE SAHÄ°BÄ° UYGULAMA REHBERÄ°"), size=10, color=GOLD, bold=True)

p = doc.add_paragraph()
p.paragraph_format.space_after = Pt(6)
set_run_font(p.add_run("Canvasia Social"), size=30, color=NAVY, bold=True)

p = doc.add_paragraph()
p.paragraph_format.space_after = Pt(20)
set_run_font(p.add_run("Instagram, Facebook, TikTok ve Pinterest resmÃ® OAuth hazÄ±rlÄ±k ve bilgi teslim kontrol listesi"), size=14, color=MID_GRAY)

add_key_value_table(doc, [
    ("HazÄ±rlayan", "Canvasia Social geliÅŸtirme Ã§alÄ±ÅŸmasÄ±"),
    ("Hedef ortam", "https://sosyalmedya.canvasia.com.tr"),
    ("Belge tarihi", "21 Temmuz 2026"),
    ("Belge amacÄ±", "Platform hesaplarÄ±nÄ± ve geliÅŸtirici uygulamalarÄ±nÄ± proje sahibinin gÃ¼venli biÃ§imde hazÄ±rlamasÄ±"),
])

doc.add_paragraph()
add_callout(
    doc,
    "En Ã¶nemli gÃ¼venlik kuralÄ±",
    "Instagram/Facebook/TikTok/Pinterest kullanÄ±cÄ± parolalarÄ±, SMS kodlarÄ±, iki aÅŸamalÄ± doÄŸrulama kodlarÄ±, kurtarma kodlarÄ± ve OAuth eriÅŸim anahtarlarÄ± geliÅŸtiriciye gÃ¶nderilmez. Client Secret deÄŸerleri mÃ¼mkÃ¼nse proje sahibi tarafÄ±ndan doÄŸrudan Railway Variables alanÄ±na girilir.",
    fill="FFF4E5",
    accent=GOLD,
)

doc.add_heading("Bu belgenin sonunda beklenen sonuÃ§", level=1)
add_bullet(doc, "Her platform iÃ§in geliÅŸtirici uygulamasÄ± oluÅŸturulmuÅŸ olacak.")
add_bullet(doc, "Production callback adresleri platform paneline eksiksiz kaydedilmiÅŸ olacak.")
add_bullet(doc, "Gerekli OAuth izinleri talep edilmiÅŸ ve test hesaplarÄ± eklenmiÅŸ olacak.")
add_bullet(doc, "GÃ¼venli ÅŸekilde teslim edilecek App ID/Client ID bilgileri hazÄ±rlanmÄ±ÅŸ olacak.")
add_bullet(doc, "Gizli deÄŸerler aÃ§Ä±k mesajla paylaÅŸÄ±lmadan Railway ortamÄ±na aktarÄ±lacak.")

add_page_break(doc)

doc.add_heading("1. Ã–nce tamamlanmasÄ± gereken ortak hazÄ±rlÄ±klar", level=1)
add_callout(
    doc,
    "Ã–nerilen uygulama sÄ±rasÄ±",
    "Ã–nce Instagram, ardÄ±ndan Facebook, sonra TikTok ve son olarak Pinterest hazÄ±rlanmalÄ±dÄ±r. Instagram ve Facebook aynÄ± Meta Business yapÄ±sÄ±ndan yÃ¶netilebilir. TikTok ve Pinterest iÃ§in ayrÄ±ca platform incelemesi gerekebilir.",
)

doc.add_heading("1.1 Proje sahibinin hazÄ±rlayacaÄŸÄ± kurumsal bilgiler", level=2)
for item in [
    "Canvasiaâ€™nÄ±n resmÃ® ÅŸirket/ticari unvanÄ± ve yetkili iletiÅŸim kiÅŸisi",
    "Kurumsal e-posta adresi",
    "https://www.canvasia.com.tr alan adÄ± Ã¼zerinde eriÅŸilebilir web sitesi",
    "Gizlilik PolitikasÄ± URLâ€™si",
    "KullanÄ±m KoÅŸullarÄ± URLâ€™si",
    "KullanÄ±cÄ± Verisi Silme TalimatÄ± veya veri silme callback URLâ€™si",
    "Uygulama adÄ±: Canvasia Social",
    "Uygulama logosu ve kÄ±sa uygulama aÃ§Ä±klamasÄ±",
    "Meta Business Portfolio ve gerekirse ÅŸirket doÄŸrulama belgeleri",
]:
    add_checkbox(doc, item)

doc.add_heading("1.2 KullanÄ±lacak production callback adresleri", level=2)
add_key_value_table(doc, [
    ("Instagram", "https://sosyalmedya.canvasia.com.tr/SocialAccounts/Instagram/Callback"),
    ("Facebook", "https://sosyalmedya.canvasia.com.tr/SocialAccounts/Facebook/Callback"),
    ("TikTok", "https://sosyalmedya.canvasia.com.tr/SocialAccounts/TikTok/Callback"),
    ("Pinterest", "https://sosyalmedya.canvasia.com.tr/SocialAccounts/Pinterest/Callback"),
], widths=(2400, 6960), header=("Platform", "Callback URL"))

doc.add_paragraph()
add_callout(
    doc,
    "Callback eÅŸleÅŸmesi",
    "Platform paneline yazÄ±lan callback ile uygulamadaki deÄŸer karakter karakter aynÄ± olmalÄ±dÄ±r. HTTPS, alan adÄ±, yol, bÃ¼yÃ¼k/kÃ¼Ã§Ã¼k harf ve sondaki eÄŸik Ã§izgi farklÄ± olmamalÄ±dÄ±r.",
    fill="FDECEC",
    accent=RED,
)

doc.add_heading("1.3 Mevcut uygulamanÄ±n gerÃ§ek durumu", level=2)
add_key_value_table(doc, [
    ("Instagram", "OAuth ve tek gÃ¶rselli normal akÄ±ÅŸ gÃ¶nderisi kodu mevcut. Story, Reels, video ve carousel henÃ¼z uygulanmadÄ±."),
    ("Facebook", "OAuth ve Facebook SayfasÄ±na tek fotoÄŸraflÄ± gÃ¶nderi kodu mevcut. KiÅŸisel profile gÃ¶nderim desteklenmez."),
    ("TikTok", "Åu anda gÃ¼venli â€˜YapÄ±landÄ±rÄ±lmadÄ±â€™ iskeleti var. OAuth ve gerÃ§ek yayÄ±n provider kodu tamamlanmadan baÄŸlantÄ± Ã§alÄ±ÅŸmaz."),
    ("Pinterest", "Åu anda gÃ¼venli â€˜YapÄ±landÄ±rÄ±lmadÄ±â€™ iskeleti var. OAuth ve gerÃ§ek Pin publisher kodu tamamlanmadan baÄŸlantÄ± Ã§alÄ±ÅŸmaz."),
], widths=(2000, 7360), header=("Platform", "Canvasia Social durumu"))

add_page_break(doc)

doc.add_heading("2. Instagram kurulumu", level=1)
add_callout(
    doc,
    "Hedef",
    "Canvasia Instagram profesyonel hesabÄ±nÄ± Instagramâ€™Ä±n resmÃ® OAuth ekranÄ± Ã¼zerinden Canvasia Socialâ€™a baÄŸlamak ve ilk aÅŸamada tek gÃ¶rselli akÄ±ÅŸ gÃ¶nderisi yayÄ±mlamak.",
)

doc.add_heading("2.1 Hesap ÅŸartlarÄ±", level=2)
for item in [
    "Instagram hesabÄ± Business veya Creator profesyonel hesaba dÃ¶nÃ¼ÅŸtÃ¼rÃ¼lmÃ¼ÅŸ olmalÄ±.",
    "Story yayÄ±nlama hedefleniyorsa Business hesap tercih edilmeli.",
    "Hesap proje sahibinin kontrolÃ¼nde olmalÄ± ve test sÃ¼recinde Meta uygulamasÄ±na eklenebilmeli.",
    "Hesap herkese aÃ§Ä±k olmalÄ±; test sÄ±rasÄ±nda hesapta ek doÄŸrulama/CAPTCHA beklememeli.",
]:
    add_checkbox(doc, item)

doc.add_heading("2.2 Meta Developer panelinde yapÄ±lacaklar", level=2)
steps = [
    "Meta for Developers portalÄ±na kurumsal olarak yÃ¶netilen Facebook/Meta hesabÄ±yla giriÅŸ yapÄ±n.",
    "Canvasia Social isimli bir Business uygulamasÄ± oluÅŸturun veya mevcut Canvasia Meta uygulamasÄ±nÄ± kullanÄ±n.",
    "Uygulamaya Instagram Ã¼rÃ¼nÃ¼nÃ¼ ekleyin ve â€˜API setup with Instagram loginâ€™ seÃ§eneÄŸini yapÄ±landÄ±rÄ±n.",
    "Canvasiaâ€™nÄ±n profesyonel Instagram hesabÄ±nÄ± test hesabÄ±/uygulama rolÃ¼ olarak ekleyin ve daveti Instagram tarafÄ±ndan kabul edin.",
    "Valid OAuth Redirect URI alanÄ±na production Instagram callback adresini aynen ekleyin.",
    "instagram_business_basic ve instagram_business_content_publish izinlerini etkinleÅŸtirin.",
    "Uygulama Development modundayken yalnÄ±zca uygulama rolÃ¼/test hesabÄ±yla OAuth testini yapÄ±n.",
    "Uygulama Canvasia dÄ±ÅŸÄ±ndaki hesaplara aÃ§Ä±lacaksa Advanced Access, App Review ve gerekirse Business Verification iÅŸlemlerini tamamlayÄ±n.",
]
for s in steps:
    add_step(doc, s)

doc.add_heading("2.3 Proje sahibinden gereken bilgiler", level=2)
add_key_value_table(doc, [
    ("Instagram App ID / Client ID", "Gizli deÄŸildir; geliÅŸtiriciye iletilebilir."),
    ("Instagram App Secret", "Gizlidir; tercihen doÄŸrudan Railway Variablesâ€™a girilmelidir."),
    ("Instagram kullanÄ±cÄ± adÄ±", "Parola olmadan yalnÄ±zca hesap doÄŸrulamasÄ± iÃ§in iletilebilir."),
    ("Uygulama modu", "Development veya Live bilgisini iletin."),
    ("Ä°zin durumu", "Standard/Advanced Access ve App Review durumunu iletin."),
    ("Callback onayÄ±", "Production callbackâ€™in Meta paneline eklendiÄŸini yazÄ±lÄ± olarak onaylayÄ±n."),
])

doc.add_heading("2.4 Railway deÄŸiÅŸkenleri", level=2)
for item in [
    "INSTAGRAM_ENABLED=true",
    "INSTAGRAM_CLIENT_ID=<Instagram App ID>",
    "INSTAGRAM_CLIENT_SECRET=<Instagram App Secret>",
    "INSTAGRAM_REDIRECT_URI=https://sosyalmedya.canvasia.com.tr/SocialAccounts/Instagram/Callback",
    "INSTAGRAM_SCOPES=instagram_business_basic,instagram_business_content_publish",
    "INSTAGRAM_API_BASE_URL=https://graph.instagram.com/v25.0/",
]:
    add_checkbox(doc, item)

doc.add_heading("2.5 Kabul testi", level=2)
for item in [
    "Sosyal Hesaplar ekranÄ±nda Instagram â€˜YapÄ±landÄ±rÄ±ldÄ±â€™ gÃ¶rÃ¼nÃ¼yor.",
    "ResmÃ® OAuth butonu Instagram yetkilendirme ekranÄ±nÄ± aÃ§Ä±yor.",
    "Callback sonrasÄ±nda hesap kullanÄ±cÄ± adÄ± uygulamada gÃ¶rÃ¼nÃ¼yor.",
    "BaÄŸlantÄ± doÄŸrulamasÄ± baÅŸarÄ±lÄ± oluyor.",
    "AUTO_PUBLISH_ENABLED=false iken tek Ã¼rÃ¼n iÃ§in taslak hazÄ±rlanÄ±yor.",
    "AyrÄ± onay sonrasÄ±nda yalnÄ±zca tek gÃ¶rselli bir test gÃ¶nderisi yayÄ±mlanÄ±yor.",
]:
    add_checkbox(doc, item)

add_page_break(doc)

doc.add_heading("3. Facebook kurulumu", level=1)
add_callout(
    doc,
    "Hedef",
    "Canvasia Facebook SayfasÄ±nÄ± resmÃ® Facebook Login/OAuth akÄ±ÅŸÄ±yla baÄŸlamak ve Sayfaya fotoÄŸraflÄ± gÃ¶nderi yayÄ±mlamak. KiÅŸisel Facebook profiline otomatik gÃ¶nderim hedeflenmemelidir.",
)

doc.add_heading("3.1 Hesap ve Sayfa ÅŸartlarÄ±", level=2)
for item in [
    "Canvasia adÄ±na yayÄ±mlanmÄ±ÅŸ bir Facebook SayfasÄ± bulunmalÄ±.",
    "OAuth iÅŸlemini yapacak kiÅŸi Sayfada tam kontrol veya iÃ§erik oluÅŸturma yetkisine sahip olmalÄ±.",
    "Sayfa bir Meta Business Portfolio iÃ§indeyse yetkiler Business Settings Ã¼zerinden doÄŸrulanmalÄ±.",
    "Birden fazla Sayfa yÃ¶netiliyorsa hedef Canvasia SayfasÄ±nÄ±n adÄ± ve Page IDâ€™si belirlenmeli.",
]:
    add_checkbox(doc, item)

doc.add_heading("3.2 Meta Developer panelinde yapÄ±lacaklar", level=2)
for s in [
    "Instagram iÃ§in kullanÄ±lan Canvasia Meta uygulamasÄ±nÄ± aÃ§Ä±n; yeni uygulama oluÅŸturmak zorunlu deÄŸildir.",
    "Facebook Login for Business Ã¼rÃ¼nÃ¼nÃ¼ ekleyin ve Web OAuth ayarlarÄ±nÄ± etkinleÅŸtirin.",
    "Valid OAuth Redirect URI alanÄ±na production Facebook callback adresini aynen ekleyin.",
    "pages_show_list, pages_read_engagement ve pages_manage_posts izinlerini yapÄ±landÄ±rÄ±n.",
    "Uygulama yÃ¶neticisinin Canvasia Facebook SayfasÄ±na eriÅŸimi olduÄŸunu doÄŸrulayÄ±n.",
    "Development modunda uygulama rolÃ¼ndeki kullanÄ±cÄ±yla Sayfa keÅŸfi ve Page access token akÄ±ÅŸÄ±nÄ± test edin.",
    "Canvasia dÄ±ÅŸÄ±ndaki kullanÄ±cÄ±lar veya Sayfalar baÄŸlanacaksa gerekli izinler iÃ§in Advanced Access/App Review baÅŸvurusu yapÄ±n.",
]:
    add_step(doc, s)

doc.add_heading("3.3 Proje sahibinden gereken bilgiler", level=2)
add_key_value_table(doc, [
    ("Meta App ID", "GeliÅŸtiriciye iletilebilir."),
    ("Meta App Secret", "Gizlidir; tercihen Railway Variablesâ€™a proje sahibi girmelidir."),
    ("Facebook Sayfa adÄ±", "Canvasia iÃ§in kullanÄ±lacak Sayfa adÄ±."),
    ("Facebook Page ID", "Varsa iletin; OAuth sonrasÄ± ayrÄ±ca doÄŸrulanacaktÄ±r."),
    ("Yetkili hesap", "Parola deÄŸil, yalnÄ±zca yetkili kiÅŸinin adÄ±/e-postasÄ± ve Sayfadaki rolÃ¼."),
    ("Ä°zin/App Review durumu", "pages_* izinlerinin eriÅŸim durumunu iletin."),
])

doc.add_heading("3.4 Railway deÄŸiÅŸkenleri", level=2)
for item in [
    "FACEBOOK_ENABLED=true",
    "FACEBOOK_CLIENT_ID=<Meta App ID>",
    "FACEBOOK_CLIENT_SECRET=<Meta App Secret>",
    "FACEBOOK_REDIRECT_URI=https://sosyalmedya.canvasia.com.tr/SocialAccounts/Facebook/Callback",
    "FACEBOOK_SCOPES=pages_show_list,pages_read_engagement,pages_manage_posts",
    "FACEBOOK_API_BASE_URL=https://graph.facebook.com/v25.0/",
]:
    add_checkbox(doc, item)

doc.add_heading("3.5 Kabul testi", level=2)
for item in [
    "Facebook â€˜YapÄ±landÄ±rÄ±ldÄ±â€™ gÃ¶rÃ¼nÃ¼yor.",
    "OAuth tamamlandÄ±ktan sonra Canvasia SayfasÄ± bulunuyor.",
    "Uygulama doÄŸru Page access tokenÄ± gÃ¼venli biÃ§imde saklÄ±yor.",
    "Sayfa CREATE_CONTENT/yayÄ±n yetkisi doÄŸrulanÄ±yor.",
    "YalnÄ±zca tek fotoÄŸraflÄ± test gÃ¶nderisi kontrollÃ¼ olarak Sayfada gÃ¶rÃ¼nÃ¼yor.",
]:
    add_checkbox(doc, item)

add_page_break(doc)

doc.add_heading("4. TikTok kurulumu", level=1)
add_callout(
    doc,
    "Mevcut yazÄ±lÄ±m sÄ±nÄ±rÄ±",
    "Canvasia Social iÃ§inde TikTok ÅŸu anda gerÃ§ek OAuth/yayÄ±n providerÄ±na sahip deÄŸildir. Proje sahibi geliÅŸtirici uygulamasÄ±nÄ± ve izinleri hazÄ±rlayabilir; ancak baÄŸlantÄ±nÄ±n Ã§alÄ±ÅŸmasÄ± iÃ§in geliÅŸtirme ekibinin TikTok provider kodunu ayrÄ±ca tamamlamasÄ± gerekir.",
    fill="FFF4E5",
    accent=GOLD,
)

doc.add_heading("4.1 Hesap ve uygulama ÅŸartlarÄ±", level=2)
for item in [
    "TikTok for Developers hesabÄ± oluÅŸturulmalÄ±.",
    "Canvasiaâ€™ya ait veya Canvasia tarafÄ±ndan yÃ¶netilen TikTok hesabÄ± hazÄ±r olmalÄ±.",
    "Kurumsal web sitesi, gizlilik politikasÄ± ve kullanÄ±m koÅŸullarÄ± eriÅŸilebilir olmalÄ±.",
    "Canvasia ve medya dosyalarÄ±nÄ±n sunulacaÄŸÄ± URL/domain sahipliÄŸi doÄŸrulanabilmeli.",
]:
    add_checkbox(doc, item)

doc.add_heading("4.2 TikTok Developer panelinde yapÄ±lacaklar", level=2)
for s in [
    "TikTok for Developers panelinde Canvasia Social isimli bir uygulama oluÅŸturun.",
    "Web platformunu ve resmÃ® Canvasia web sitesi URLâ€™sini ekleyin.",
    "Login Kit Ã¼rÃ¼nÃ¼nÃ¼ etkinleÅŸtirin.",
    "Content Posting API Ã¼rÃ¼nÃ¼nÃ¼ ekleyin ve Direct Post Ã¶zelliÄŸini yapÄ±landÄ±rÄ±n.",
    "Production TikTok callback adresini Redirect URI olarak aynen ekleyin.",
    "user.info.basic ve video.publish scopeâ€™larÄ±nÄ± talep edin.",
    "Canvasia Ã¼rÃ¼n gÃ¶rsellerinin/video dosyalarÄ±nÄ±n okunacaÄŸÄ± domain veya URL prefix sahipliÄŸini doÄŸrulayÄ±n.",
    "Sandbox/Development ortamÄ±nda OAuth ve Ã¶zel gÃ¶rÃ¼nÃ¼rlÃ¼kte gÃ¶nderi testini tamamlayÄ±n.",
    "GÃ¶nderilerin herkese aÃ§Ä±k olabilmesi iÃ§in Content Posting API audit baÅŸvurusunu tamamlayÄ±n.",
]:
    add_step(doc, s)

add_callout(
    doc,
    "TikTok denetim kÄ±sÄ±tÄ±",
    "DenetlenmemiÅŸ API istemcilerinden Direct Post ile gÃ¶nderilen iÃ§erikler Ã¶zel (SELF_ONLY) gÃ¶rÃ¼nÃ¼rlÃ¼kle sÄ±nÄ±rlandÄ±rÄ±lÄ±r. Herkese aÃ§Ä±k otomatik yayÄ±n iÃ§in TikTok audit/onayÄ± gereklidir.",
    fill="FDECEC",
    accent=RED,
)

doc.add_heading("4.3 Proje sahibinden gereken bilgiler", level=2)
add_key_value_table(doc, [
    ("TikTok Client Key", "GeliÅŸtiriciye iletilebilir."),
    ("TikTok Client Secret", "Gizlidir; Railway Variablesâ€™a doÄŸrudan girilmelidir."),
    ("TikTok kullanÄ±cÄ± adÄ±", "Parola olmadan hesap doÄŸrulamasÄ± iÃ§in iletilebilir."),
    ("Content Posting API", "Etkin/onay bekliyor/onaylÄ± durumunu iletin."),
    ("video.publish", "Scope eriÅŸim durumunu iletin."),
    ("Domain doÄŸrulamasÄ±", "DoÄŸrulanan domain ve URL prefix listesini iletin."),
    ("Audit durumu", "DenetlenmemiÅŸ, incelemede veya onaylÄ±."),
])

doc.add_heading("4.4 Planlanan Railway deÄŸiÅŸkenleri", level=2)
for item in [
    "TIKTOK_ENABLED=true (provider kodu tamamlandÄ±ktan sonra)",
    "TIKTOK_CLIENT_ID=<TikTok Client Key>",
    "TIKTOK_CLIENT_SECRET=<TikTok Client Secret>",
    "TIKTOK_REDIRECT_URI=https://sosyalmedya.canvasia.com.tr/SocialAccounts/TikTok/Callback",
    "TIKTOK_SCOPES=user.info.basic,video.publish",
    "TIKTOK_API_BASE_URL=https://open.tiktokapis.com/v2/",
]:
    add_checkbox(doc, item)

doc.add_heading("4.5 GeliÅŸtirme ekibinin tamamlayacaÄŸÄ± iÅŸler", level=2)
for item in [
    "TikTok OAuth authorization ve callback akÄ±ÅŸÄ±",
    "Access token ve refresh token yenileme",
    "Creator info sorgulama ve kullanÄ±cÄ± onayÄ± arayÃ¼zÃ¼",
    "FotoÄŸraf/video Direct Post baÅŸlatma",
    "Dosya aktarÄ±mÄ± veya doÄŸrulanmÄ±ÅŸ URLâ€™den Ã§ekme",
    "YayÄ±n durumunu polling/webhook ile takip etme",
    "TikTok hata kodlarÄ±, rate limit ve audit kÄ±sÄ±tlarÄ±",
]:
    add_checkbox(doc, item)

add_page_break(doc)

doc.add_heading("5. Pinterest kurulumu", level=1)
add_callout(
    doc,
    "Mevcut yazÄ±lÄ±m sÄ±nÄ±rÄ±",
    "Canvasia Social iÃ§inde Pinterest ÅŸu anda gerÃ§ek OAuth/yayÄ±n providerÄ±na sahip deÄŸildir. Proje sahibi Pinterest geliÅŸtirici uygulamasÄ±nÄ± hazÄ±rlayabilir; gerÃ§ek Pin oluÅŸturma iÃ§in provider kodu ayrÄ±ca tamamlanmalÄ±dÄ±r.",
    fill="FFF4E5",
    accent=GOLD,
)

doc.add_heading("5.1 Hesap ÅŸartlarÄ±", level=2)
for item in [
    "Pinterest Business hesabÄ± oluÅŸturulmalÄ±.",
    "HesabÄ±n e-posta adresi doÄŸrulanmalÄ±.",
    "Pinterest Developer Terms kabul edilmeli.",
    "ÃœrÃ¼nlerin gÃ¶nderileceÄŸi en az bir Pinterest panosu hazÄ±rlanmalÄ±.",
]:
    add_checkbox(doc, item)

doc.add_heading("5.2 Pinterest Developer panelinde yapÄ±lacaklar", level=2)
for s in [
    "Pinterest Developer portalÄ±nda â€˜Connect appâ€™ adÄ±mlarÄ±nÄ± baÅŸlatÄ±n.",
    "Canvasia Social uygulama bilgilerini ve kullanÄ±m amacÄ±nÄ± girin.",
    "Trial access baÅŸvurusunu gÃ¶nderin ve inceleme sonucunu bekleyin.",
    "Uygulama onaylandÄ±ktan sonra App ID ve App Secret bilgilerini alÄ±n.",
    "Production Pinterest callback adresini Redirect URIs alanÄ±na aynen ekleyin.",
    "user_accounts:read, boards:read, pins:read ve pins:write scopeâ€™larÄ±nÄ± yapÄ±landÄ±rÄ±n.",
    "OAuth Authorization Code akÄ±ÅŸÄ±nÄ± kullanÄ±n.",
    "Pinterest hesabÄ±ndaki hedef Canvasia panosunu belirleyin.",
    "Uygulama yalnÄ±zca sahibi dÄ±ÅŸÄ±ndaki kullanÄ±cÄ±lara da aÃ§Ä±lacaksa Standard access baÅŸvurusunu planlayÄ±n.",
]:
    add_step(doc, s)

add_callout(
    doc,
    "Pinterest eriÅŸim seviyesi",
    "Trial access geliÅŸtirme ve deneme iÃ§indir; Trial ile oluÅŸturulan Pinler yalnÄ±zca uygulama sahibine gÃ¶rÃ¼nÃ¼r olabilir. Genel kullanÄ±m ve tam gÃ¶rÃ¼nÃ¼rlÃ¼k iÃ§in Standard access gerekebilir.",
    fill="FDECEC",
    accent=RED,
)

doc.add_heading("5.3 Proje sahibinden gereken bilgiler", level=2)
add_key_value_table(doc, [
    ("Pinterest App ID", "GeliÅŸtiriciye iletilebilir."),
    ("Pinterest App Secret", "Gizlidir; Railway Variablesâ€™a doÄŸrudan girilmelidir."),
    ("Pinterest kullanÄ±cÄ± adÄ±", "Parola olmadan iletilebilir."),
    ("Hedef pano", "Pano adÄ± ve mÃ¼mkÃ¼nse Board ID."),
    ("EriÅŸim seviyesi", "Trial veya Standard."),
    ("BaÅŸvuru durumu", "Bekliyor, reddedildi veya onaylandÄ±."),
    ("Callback onayÄ±", "Production callbackâ€™in eklendiÄŸini onaylayÄ±n."),
])

doc.add_heading("5.4 Planlanan Railway deÄŸiÅŸkenleri", level=2)
for item in [
    "PINTEREST_ENABLED=true (provider kodu tamamlandÄ±ktan sonra)",
    "PINTEREST_CLIENT_ID=<Pinterest App ID>",
    "PINTEREST_CLIENT_SECRET=<Pinterest App Secret>",
    "PINTEREST_REDIRECT_URI=https://sosyalmedya.canvasia.com.tr/SocialAccounts/Pinterest/Callback",
    "PINTEREST_SCOPES=user_accounts:read,boards:read,pins:read,pins:write",
    "PINTEREST_API_BASE_URL=https://api.pinterest.com/v5/",
]:
    add_checkbox(doc, item)

doc.add_heading("5.5 GeliÅŸtirme ekibinin tamamlayacaÄŸÄ± iÅŸler", level=2)
for item in [
    "Pinterest OAuth authorization/callback ve state doÄŸrulamasÄ±",
    "Access/refresh token saklama ve yenileme",
    "Pano listeleme ve hedef pano seÃ§imi",
    "GÃ¶rsel veya video Pin oluÅŸturma",
    "ÃœrÃ¼n baÅŸlÄ±ÄŸÄ±, aÃ§Ä±klama ve Canvasia Ã¼rÃ¼n baÄŸlantÄ±sÄ± ekleme",
    "Pin ID/URL, hata ve rate limit takibi",
]:
    add_checkbox(doc, item)

add_page_break(doc)

doc.add_heading("6. Bilgilerin gÃ¼venli teslimi", level=1)
add_callout(
    doc,
    "Ã–nerilen yÃ¶ntem",
    "En gÃ¼venli yÃ¶ntem, proje sahibinin ilgili platform uygulamalarÄ±nda geliÅŸtiriciye rol vermesi ve Client Secret deÄŸerlerini doÄŸrudan Railway Variablesâ€™a kendisinin girmesidir. BÃ¶ylece gizli deÄŸer sohbet, e-posta veya dosyada dolaÅŸmaz.",
    fill="EAF7F0",
    accent=GREEN,
)

doc.add_heading("6.1 Normal ÅŸekilde iletilebilecek bilgiler", level=2)
for item in [
    "App ID, Client ID veya Client Key",
    "Platform kullanÄ±cÄ± adÄ± ve hesap/Sayfa adÄ±",
    "Facebook Page ID ve Pinterest Board ID",
    "Callback URLâ€™nin kaydedildiÄŸine iliÅŸkin onay",
    "UygulamanÄ±n Development/Live durumu",
    "Ä°zin, App Review, audit ve eriÅŸim seviyesi durumlarÄ±",
    "DoÄŸrulanmÄ±ÅŸ domainlerin listesi",
]:
    add_bullet(doc, item)

doc.add_heading("6.2 YalnÄ±zca gÃ¼venli kanalla aktarÄ±lacak bilgiler", level=2)
for item in [
    "Instagram/Meta App Secret",
    "Facebook App Secret",
    "TikTok Client Secret",
    "Pinterest App Secret",
]:
    add_bullet(doc, item)

doc.add_heading("6.3 Kesinlikle gÃ¶nderilmeyecek bilgiler", level=2)
for item in [
    "Sosyal medya hesap parolalarÄ±",
    "SMS/e-posta doÄŸrulama kodlarÄ±",
    "Ä°ki aÅŸamalÄ± doÄŸrulama kodlarÄ±",
    "Kurtarma kodlarÄ±",
    "OAuth access token veya refresh token deÄŸerleri",
    "KiÅŸisel Facebook/Meta hesap parolasÄ±",
]:
    add_bullet(doc, item)

doc.add_heading("6.4 Railwayâ€™de girilecek ortak gÃ¼venlik deÄŸerleri", level=2)
for item in [
    "AUTO_PUBLISH_ENABLED=false (ilk baÄŸlantÄ± ve taslak testleri boyunca)",
    "TRUST_FORWARDED_HEADERS=true (Railway reverse proxy yapÄ±landÄ±rmasÄ± doÄŸrulandÄ±ktan sonra)",
    "Security__UseHttpsRedirection=true",
    "DATA_PROTECTION_STORE=database veya production iÃ§in belirlenen kalÄ±cÄ± gÃ¼venli yÃ¶ntem",
    "CANVASIA_ALLOWED_IMAGE_HOSTS=canvasia.com.tr,www.canvasia.com.tr",
]:
    add_checkbox(doc, item)

add_page_break(doc)

doc.add_heading("7. Proje sahibinin dolduracaÄŸÄ± teslim formu", level=1)
add_callout(
    doc,
    "Formun kullanÄ±mÄ±",
    "Gizli alanlara Client Secret yazmayÄ±n. Bu alanlarda yalnÄ±zca â€˜Railwayâ€™e girildiâ€™ veya â€˜GÃ¼venli kasa baÄŸlantÄ±sÄ± paylaÅŸÄ±ldÄ±â€™ ÅŸeklinde durum belirtin.",
)

form_rows = [
    ("Gizlilik PolitikasÄ± URL", ""),
    ("KullanÄ±m KoÅŸullarÄ± URL", ""),
    ("Veri Silme URL/TalimatÄ±", ""),
    ("Meta Business Portfolio ID", ""),
    ("Instagram kullanÄ±cÄ± adÄ±", ""),
    ("Instagram App ID", ""),
    ("Instagram App Secret durumu", "Railwayâ€™e girildi / Bekliyor"),
    ("Instagram App Review durumu", ""),
    ("Facebook Sayfa adÄ±", ""),
    ("Facebook Page ID", ""),
    ("Meta App ID", ""),
    ("Meta App Secret durumu", "Railwayâ€™e girildi / Bekliyor"),
    ("Facebook izin durumu", ""),
    ("TikTok kullanÄ±cÄ± adÄ±", ""),
    ("TikTok Client Key", ""),
    ("TikTok Client Secret durumu", "Railwayâ€™e girildi / Bekliyor"),
    ("TikTok Content Posting audit", ""),
    ("TikTok doÄŸrulanan domainler", ""),
    ("Pinterest kullanÄ±cÄ± adÄ±", ""),
    ("Pinterest App ID", ""),
    ("Pinterest App Secret durumu", "Railwayâ€™e girildi / Bekliyor"),
    ("Pinterest eriÅŸim seviyesi", "Trial / Standard / Bekliyor"),
    ("Pinterest hedef pano", ""),
]
add_key_value_table(doc, form_rows, widths=(3600, 5760), header=("Bilgi", "Proje sahibi yanÄ±tÄ±"))

add_page_break(doc)

doc.add_heading("8. Son kabul sÄ±rasÄ±", level=1)
for item in [
    "Production alan adÄ± ve HTTPS Ã§alÄ±ÅŸÄ±yor.",
    "Platform callback URLâ€™leri platform panellerine eksiksiz kaydedildi.",
    "Railway secret deÄŸiÅŸkenleri gÃ¼venli biÃ§imde girildi.",
    "Web ve Worker yeniden baÅŸlatÄ±ldÄ± ve health kontrolleri baÅŸarÄ±lÄ±.",
    "Sosyal Hesaplar ekranÄ±nda ilgili platform â€˜YapÄ±landÄ±rÄ±ldÄ±â€™ gÃ¶rÃ¼nÃ¼yor.",
    "OAuth yalnÄ±zca platformun resmÃ® ekranÄ±nda tamamlandÄ±.",
    "BaÄŸlanan hesabÄ±n kullanÄ±cÄ± adÄ±/SayfasÄ± doÄŸrulandÄ±.",
    "AUTO_PUBLISH_ENABLED=false iken taslak ve planlama testi yapÄ±ldÄ±.",
    "Bekleyen baÅŸka planlanmÄ±ÅŸ iÃ§erik olmadÄ±ÄŸÄ± doÄŸrulandÄ±.",
    "KullanÄ±cÄ± onayÄ±yla yalnÄ±zca tek bir gerÃ§ek gÃ¶nderi test edildi.",
    "GerÃ§ek platform post ID/URL ve uygulama yayÄ±n geÃ§miÅŸi birlikte doÄŸrulandÄ±.",
    "Test sonrasÄ±nda otomatik yayÄ±n politikasÄ± ve gÃ¼nlÃ¼k limitler ayrÄ±ca onaylandÄ±.",
]:
    add_checkbox(doc, item)

doc.add_heading("9. ResmÃ® kaynaklar", level=1)
add_source(doc, "Meta for Developers - Uygulamalar", "https://developers.facebook.com/apps/")
add_source(doc, "Meta - Instagram API with Instagram Login", "https://www.postman.com/meta/instagram/folder/6raa77c/instagram-api-with-instagram-login")
add_source(doc, "Meta - Instagram API dokÃ¼mantasyonu", "https://www.postman.com/meta/instagram/documentation/6yqw8pt/instagram-api")
add_source(doc, "Meta - Facebook API resmÃ® Ã§alÄ±ÅŸma alanÄ±", "https://www.postman.com/meta/")
add_source(doc, "Facebook - Sayfa eriÅŸimi hakkÄ±nda", "https://www.facebook.com/help/289207354498410/")
add_source(doc, "TikTok - Uygulama oluÅŸturma", "https://developers.tiktok.com/doc/getting-started-create-an-app/")
add_source(doc, "TikTok - Login Kit", "https://developers.tiktok.com/doc/login-kit-overview/")
add_source(doc, "TikTok - Content Posting API baÅŸlangÄ±Ã§", "https://developers.tiktok.com/doc/content-posting-api-get-started/")
add_source(doc, "TikTok - Content Sharing Guidelines", "https://developers.tiktok.com/doc/content-sharing-guidelines/")
add_source(doc, "Pinterest - Uygulama baÄŸlama", "https://developers.pinterest.com/docs/getting-started/connect-app/")
add_source(doc, "Pinterest - OAuth ve yetkilendirme", "https://developers.pinterest.com/docs/getting-started/set-up-authentication-and-authorization/")
add_source(doc, "Pinterest - Board ve Pin oluÅŸturma", "https://developers.pinterest.com/docs/work-with-organic-content-and-users/create-boards-and-pins/")
add_source(doc, "Pinterest - EriÅŸim seviyeleri", "https://developers.pinterest.com/docs/key-concepts/access-tiers/")

p = doc.add_paragraph()
p.paragraph_format.space_before = Pt(12)
p.paragraph_format.space_after = Pt(0)
set_run_font(p.add_run("Not: Platform panelleri, izin adlarÄ± ve inceleme koÅŸullarÄ± zamanla deÄŸiÅŸebilir. BaÅŸvuru sÄ±rasÄ±nda ilgili platformun gÃ¼ncel resmÃ® paneli ve dokÃ¼mantasyonu esas alÄ±nmalÄ±dÄ±r."), size=9.5, color=MID_GRAY, italic=True)

doc.core_properties.title = "Canvasia Social - Sosyal Medya OAuth Kurulum Rehberi"
doc.core_properties.subject = "Instagram, Facebook, TikTok ve Pinterest OAuth hazÄ±rlÄ±k kontrol listesi"
doc.core_properties.author = "Canvasia Social"
doc.core_properties.keywords = "Canvasia, OAuth, Instagram, Facebook, TikTok, Pinterest"

OUT.parent.mkdir(parents=True, exist_ok=True)
doc.save(OUT)
print(OUT)

