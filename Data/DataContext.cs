using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using complaints_back.models;
using complaints_back.models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace complaints_back.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categories>().HasData(
                new Categories
                {
                    Id = 1,
                    Name = "Theft",
                    Description = "Taking someone else's property without permission or legal right.",
                    AR_Name = "سرقة",
                    AR_Des = "أخذ ممتلكات شخص آخر دون إذن أو حق قانوني."
                },
                new Categories
                {
                    Id = 2,
                    Name = "Harassment",
                    Description = "Offensive, humiliating, or threatening behavior targeted at an individual.",
                    AR_Name = "تحرش",
                    AR_Des = "سلوك مسيء أو مهين أو مهدد يستهدف فردًا."
                },
                new Categories
                {
                    Id = 3,
                    Name = "Assault",
                    Description = "Physical attack or threat of violence against a person.",
                    AR_Name = "اعتداء",
                    AR_Des = "هجوم جسدي أو تهديد بالعنف ضد شخص."
                },
                new Categories
                {
                    Id = 4,
                    Name = "Vandalism",
                    Description = "Deliberate destruction or damage to property.",
                    AR_Name = "تخريب",
                    AR_Des = "تدمير أو إتلاف متعمد للممتلكات."
                },
                new Categories
                {
                    Id = 5,
                    Name = "Discrimination",
                    Description = "Unfair treatment based on race, gender, religion, disability, or other protected characteristics.",
                    AR_Name = "تمييز",
                    AR_Des = "معاملة غير عادلة بناءً على العرق أو الجنس أو الدين أو الإعاقة أو غيرها من الخصائص المحمية."
                },
                new Categories
                {
                    Id = 6,
                    Name = "Bullying",
                    Description = "Intentional intimidation, abuse, or humiliation of another person.",
                    AR_Name = "تنمر",
                    AR_Des = "تخويف أو إساءة أو إذلال متعمد لشخص آخر."
                },
                new Categories
                {
                    Id = 7,
                    Name = "Sexual Misconduct",
                    Description = "Unwelcome sexual behavior, including harassment, assault, or exploitation.",
                    AR_Name = "سوء سلوك جنسي",
                    AR_Des = "سلوك جنسي غير مرغوب فيه، بما في ذلك التحرش أو الاعتداء أو الاستغلال."
                },
                new Categories
                {
                    Id = 8,
                    Name = "Corruption",
                    Description = "Abuse of entrusted power for private gain.",
                    AR_Name = "فساد",
                    AR_Des = "إساءة استخدام السلطة الموكلة لتحقيق مكاسب شخصية."
                },
                new Categories
                {
                    Id = 9,
                    Name = "Noise Complaint",
                    Description = "Disturbance due to loud music, parties, construction, etc.",
                    AR_Name = "شكوى ضوضاء",
                    AR_Des = "إزعاج بسبب الموسيقى الصاخبة أو الحفلات أو البناء وما إلى ذلك."
                },
                new Categories
                {
                    Id = 10,
                    Name = "Littering",
                    Description = "Improper disposal of waste in public or private areas.",
                    AR_Name = "رمي النفايات",
                    AR_Des = "التخلص غير السليم من النفايات في الأماكن العامة أو الخاصة."
                },
                new Categories
                {
                    Id = 11,
                    Name = "Traffic Violation",
                    Description = "Unsafe or illegal actions by drivers, such as speeding or ignoring signals.",
                    AR_Name = "مخالفة مرورية",
                    AR_Des = "أفعال غير آمنة أو غير قانونية من قبل السائقين مثل السرعة أو تجاهل الإشارات."
                },
                new Categories
                {
                    Id = 12,
                    Name = "Trespassing",
                    Description = "Entering someone's property without permission.",
                    AR_Name = "تعدي",
                    AR_Des = "دخول ممتلكات شخص ما دون إذن."
                },
                new Categories
                {
                    Id = 13,
                    Name = "Cyberbullying",
                    Description = "Harassment or abuse through digital communication.",
                    AR_Name = "تنمر إلكتروني",
                    AR_Des = "تحرش أو إساءة من خلال وسائل الاتصال الرقمية."
                },
                new Categories
                {
                    Id = 14,
                    Name = "Fraud",
                    Description = "Wrongful or criminal deception intended to result in financial or personal gain.",
                    AR_Name = "احتيال",
                    AR_Des = "خداع غير قانوني أو إجرامي يهدف لتحقيق مكاسب مالية أو شخصية."
                },
                new Categories
                {
                    Id = 15,
                    Name = "Substance Abuse",
                    Description = "Illegal use or possession of drugs or alcohol.",
                    AR_Name = "تعاطي مواد",
                    AR_Des = "استخدام أو حيازة غير قانونية للمخدرات أو الكحول."
                },
                new Categories
                {
                    Id = 16,
                    Name = "Bribery",
                    Description = "Offering or accepting something of value in exchange for influence or action.",
                    AR_Name = "رشوة",
                    AR_Des = "تقديم أو قبول شيء ذي قيمة مقابل التأثير أو اتخاذ إجراء."
                },
                new Categories
                {
                    Id = 17,
                    Name = "Unethical Behavior",
                    Description = "Actions that violate moral or professional standards.",
                    AR_Name = "سلوك غير أخلاقي",
                    AR_Des = "أفعال تنتهك المعايير الأخلاقية أو المهنية."
                },
                new Categories
                {
                    Id = 18,
                    Name = "Workplace Violence",
                    Description = "Threats or acts of violence in a professional environment.",
                    AR_Name = "عنف في مكان العمل",
                    AR_Des = "تهديدات أو أعمال عنف في بيئة مهنية."
                },
                new Categories
                {
                    Id = 19,
                    Name = "Property Damage",
                    Description = "Destruction or defacement of personal or public property.",
                    AR_Name = "تلف الممتلكات",
                    AR_Des = "تدمير أو تشويه الممتلكات الشخصية أو العامة."
                },
                new Categories
                {
                    Id = 20,
                    Name = "Other",
                    Description = "Any complaint that does not fit in the predefined categories.",
                    AR_Name = "أخرى",
                    AR_Des = "أي شكوى لا تندرج ضمن الفئات المحددة مسبقًا."
                }
            );
        }
    }
}