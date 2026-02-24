using BlazorApp.Models;
using ClosedXML.Excel;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlazorApp.Services
{
    public class ExportService
    {
        // Excel Export
        public byte[] ExportToExcel(List<Member> members)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Members");

            // Title
            worksheet.Cell(1, 1).Value = "Member Registration Report";
            worksheet.Range(1, 1, 1, 6).Merge();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Date
            worksheet.Cell(2, 1).Value = $"Exported on: {DateTime.Now:yyyy-MM-dd HH:mm}";
            worksheet.Range(2, 1, 2, 6).Merge();
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers
            var headers = new[] { "Id", "Full Name", "Email", "Phone", "Membership Type", "Registered Date" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data
            for (int i = 0; i < members.Count; i++)
            {
                var m = members[i];
                worksheet.Cell(i + 5, 1).Value = m.Id;
                worksheet.Cell(i + 5, 2).Value = m.FullName;
                worksheet.Cell(i + 5, 3).Value = m.Email;
                worksheet.Cell(i + 5, 4).Value = m.Phone;
                worksheet.Cell(i + 5, 5).Value = m.MembershipType;
                worksheet.Cell(i + 5, 6).Value = m.RegisteredDate.ToString("yyyy-MM-dd");

                // Alternate row color
                if (i % 2 == 0)
                    worksheet.Row(i + 5).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // PDF Export — using HTML to PDF approach
        public byte[] ExportToPdf(List<Member> members)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Text("Member Registration Report")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken3)
                            .AlignCenter();
                        col.Item().Text($"Exported on: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Medium)
                            .AlignCenter();
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // Content
                    page.Content().PaddingTop(20).Table(table =>
                    {
                        // Column definitions
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // Id
                            columns.RelativeColumn(2);   // Full Name
                            columns.RelativeColumn(3);   // Email
                            columns.RelativeColumn(2);   // Phone
                            columns.RelativeColumn(2);   // Membership
                            columns.RelativeColumn(2);   // Date
                        });

                        // Table Header
                        static IContainer HeaderStyle(IContainer c) => c
                            .Background(Colors.Blue.Darken3)
                            .Padding(6)
                            .AlignMiddle();

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Id").FontColor(Colors.White).Bold();
                            header.Cell().Element(HeaderStyle).Text("Full Name").FontColor(Colors.White).Bold();
                            header.Cell().Element(HeaderStyle).Text("Email").FontColor(Colors.White).Bold();
                            header.Cell().Element(HeaderStyle).Text("Phone").FontColor(Colors.White).Bold();
                            header.Cell().Element(HeaderStyle).Text("Membership").FontColor(Colors.White).Bold();
                            header.Cell().Element(HeaderStyle).Text("Registered").FontColor(Colors.White).Bold();
                        });

                        // Table Rows
                        for (int i = 0; i < members.Count; i++)
                        {
                            var m = members[i];
                            var bgColor = i % 2 == 0 ? Colors.White : Colors.Blue.Lighten5;

                            static IContainer CellStyle(IContainer c) => c.Padding(6).AlignMiddle();

                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.Id.ToString());
                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.FullName);
                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.Email);
                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.Phone);
                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.MembershipType);
                            table.Cell().Background(bgColor).Element(CellStyle).Text(m.RegisteredDate.ToString("yyyy-MM-dd"));
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}