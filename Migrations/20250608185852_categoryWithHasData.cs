using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace complaints_back.Migrations
{
    /// <inheritdoc />
    public partial class categoryWithHasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AR_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AR_Des = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "AR_Des", "AR_Name", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "أخذ ممتلكات شخص آخر دون إذن أو حق قانوني.", "سرقة", "Taking someone else's property without permission or legal right.", "Theft" },
                    { 2, "سلوك مسيء أو مهين أو مهدد يستهدف فردًا.", "تحرش", "Offensive, humiliating, or threatening behavior targeted at an individual.", "Harassment" },
                    { 3, "هجوم جسدي أو تهديد بالعنف ضد شخص.", "اعتداء", "Physical attack or threat of violence against a person.", "Assault" },
                    { 4, "تدمير أو إتلاف متعمد للممتلكات.", "تخريب", "Deliberate destruction or damage to property.", "Vandalism" },
                    { 5, "معاملة غير عادلة بناءً على العرق أو الجنس أو الدين أو الإعاقة أو غيرها من الخصائص المحمية.", "تمييز", "Unfair treatment based on race, gender, religion, disability, or other protected characteristics.", "Discrimination" },
                    { 6, "تخويف أو إساءة أو إذلال متعمد لشخص آخر.", "تنمر", "Intentional intimidation, abuse, or humiliation of another person.", "Bullying" },
                    { 7, "سلوك جنسي غير مرغوب فيه، بما في ذلك التحرش أو الاعتداء أو الاستغلال.", "سوء سلوك جنسي", "Unwelcome sexual behavior, including harassment, assault, or exploitation.", "Sexual Misconduct" },
                    { 8, "إساءة استخدام السلطة الموكلة لتحقيق مكاسب شخصية.", "فساد", "Abuse of entrusted power for private gain.", "Corruption" },
                    { 9, "إزعاج بسبب الموسيقى الصاخبة أو الحفلات أو البناء وما إلى ذلك.", "شكوى ضوضاء", "Disturbance due to loud music, parties, construction, etc.", "Noise Complaint" },
                    { 10, "التخلص غير السليم من النفايات في الأماكن العامة أو الخاصة.", "رمي النفايات", "Improper disposal of waste in public or private areas.", "Littering" },
                    { 11, "أفعال غير آمنة أو غير قانونية من قبل السائقين مثل السرعة أو تجاهل الإشارات.", "مخالفة مرورية", "Unsafe or illegal actions by drivers, such as speeding or ignoring signals.", "Traffic Violation" },
                    { 12, "دخول ممتلكات شخص ما دون إذن.", "تعدي", "Entering someone's property without permission.", "Trespassing" },
                    { 13, "تحرش أو إساءة من خلال وسائل الاتصال الرقمية.", "تنمر إلكتروني", "Harassment or abuse through digital communication.", "Cyberbullying" },
                    { 14, "خداع غير قانوني أو إجرامي يهدف لتحقيق مكاسب مالية أو شخصية.", "احتيال", "Wrongful or criminal deception intended to result in financial or personal gain.", "Fraud" },
                    { 15, "استخدام أو حيازة غير قانونية للمخدرات أو الكحول.", "تعاطي مواد", "Illegal use or possession of drugs or alcohol.", "Substance Abuse" },
                    { 16, "تقديم أو قبول شيء ذي قيمة مقابل التأثير أو اتخاذ إجراء.", "رشوة", "Offering or accepting something of value in exchange for influence or action.", "Bribery" },
                    { 17, "أفعال تنتهك المعايير الأخلاقية أو المهنية.", "سلوك غير أخلاقي", "Actions that violate moral or professional standards.", "Unethical Behavior" },
                    { 18, "تهديدات أو أعمال عنف في بيئة مهنية.", "عنف في مكان العمل", "Threats or acts of violence in a professional environment.", "Workplace Violence" },
                    { 19, "تدمير أو تشويه الممتلكات الشخصية أو العامة.", "تلف الممتلكات", "Destruction or defacement of personal or public property.", "Property Damage" },
                    { 20, "أي شكوى لا تندرج ضمن الفئات المحددة مسبقًا.", "أخرى", "Any complaint that does not fit in the predefined categories.", "Other" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
