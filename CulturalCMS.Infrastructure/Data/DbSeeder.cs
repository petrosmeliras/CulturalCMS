using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(CulturalDbContext context)
        {
            // Αν υπάρχουν ήδη ρόλοι, η βάση έχει δεδομένα, οπότε σταματάμε το seeding
            if (await context.Roles.AnyAsync()) return;

            // --- 1. Δημιουργία Ρόλων ---
            var adminRole = new Role { Name = "Admin" };
            var curatorRole = new Role { Name = "Curator" };
            var contributorRole = new Role { Name = "Contributor" };
            var publicRole = new Role { Name = "PublicUser" };

            await context.Roles.AddRangeAsync(adminRole, curatorRole, contributorRole, publicRole);
            await context.SaveChangesAsync();

            // --- 2. Δημιουργία Χρηστών με Hashed Passwords ---
            var adminUser = new User { Username = "admin", Email = "admin@museum.gr", Firstname = "Γιώργος", Lastname = "Διαχειριστής", RoleId = adminRole.Id, Password = BCrypt.Net.BCrypt.HashPassword("Admin123!") };
            var curatorUser = new User { Username = "curator", Email = "curator@museum.gr", Firstname = "Μαρία", Lastname = "Επιμελήτρια", RoleId = curatorRole.Id, Password = BCrypt.Net.BCrypt.HashPassword("Curator123!") };
            var contributorUser = new User { Username = "contributor", Email = "contrib@museum.gr", Firstname = "Κώστας", Lastname = "Δημιουργός", RoleId = contributorRole.Id, Password = BCrypt.Net.BCrypt.HashPassword("Contrib123!") };
            var contributorUser2 = new User { Username = "contributor2", Email = "contrib2@museum.gr", Firstname = "Ελένη", Lastname = "Παπαδάκη", RoleId = contributorRole.Id, Password = BCrypt.Net.BCrypt.HashPassword("Contrib123!") };
            var publicUser = new User { Username = "visitor", Email = "visitor@museum.gr", Firstname = "Νίκος", Lastname = "Επισκέπτης", RoleId = publicRole.Id, Password = BCrypt.Net.BCrypt.HashPassword("Visitor123!") };

            await context.Users.AddRangeAsync(adminUser, curatorUser, contributorUser, contributorUser2, publicUser);
            await context.SaveChangesAsync();

            // --- 3. Δημιουργία Τεκμηρίων & Μεταδεδομένων ---
            // Ποικιλία τύπων (εκθέματα, ιστορικά έγγραφα, έργα τέχνης, μνημεία) και πλήρης κάλυψη
            // των παραδειγμάτων μεταδεδομένων της εκφώνησης: Υλικό, Διαστάσεις, Τοποθεσία Εύρεσης,
            // Τεχνοτροπία, Δημιουργός/Καλλιτέχνης, Tags.
            var items = new List<CulturalItem>
            {
                // ── ΓΛΥΠΤΑ / ΕΚΘΕΜΑΤΑ ──────────────────────────────────────────
                new CulturalItem
                {
                    Title = "Ποσειδώνας του Αρτεμισίου",
                    Description = "Χάλκινο άγαλμα της κλασικής περιόδου, βρέθηκε στον βυθό κοντά στο Αρτεμίσιο.",
                    Category = "Γλυπτό", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(209, 0, 0, "cm"),
                    Coordinates = new Coordinates(38.9836, 23.0022),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Χαλκός" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Ακρωτήριο Αρτεμίσιο, Εύβοια" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Αυστηρός Ρυθμός" },
                        new ItemMetadata { Key = "Tag", Value = "Άγαλμα" },
                        new ItemMetadata { Key = "Tag", Value = "Θαλάσσια Αρχαιολογία" }
                    }
                },
                new CulturalItem
                {
                    Title = "Αφροδίτη της Μήλου",
                    Description = "Περίφημο μαρμάρινο άγαλμα, σύμβολο της ελληνιστικής γλυπτικής.",
                    Category = "Γλυπτό", HistoricalPeriod = "Ελληνιστική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(203, 0, 0, "cm"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Μήλος" },
                        new ItemMetadata { Key = "Δημιουργός/Καλλιτέχνης", Value = "Αλέξανδρος της Αντιόχειας (αποδιδόμενο)" },
                        new ItemMetadata { Key = "Tag", Value = "Άγαλμα" }
                    }
                },
                new CulturalItem
                {
                    Title = "Κούρος της Βολομάνδρας",
                    Description = "Αρχαϊκό άγαλμα νέου άνδρα, χαρακτηριστικό παράδειγμα πρώιμης ελληνικής γλυπτικής.",
                    Category = "Γλυπτό", HistoricalPeriod = "Αρχαϊκή Περίοδος",
                    Status = ItemStatus.ForReview, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(180, 0, 0, "cm"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Αρχαϊκή Τεχνοτροπία" },
                        new ItemMetadata { Key = "Tag", Value = "Άγαλμα" }
                    }
                },
                new CulturalItem
                {
                    Title = "Νίκη της Σαμοθράκης",
                    Description = "Μαρμάρινο γλυπτό της θεάς Νίκης, ένα από τα σημαντικότερα έργα της ελληνιστικής τέχνης.",
                    Category = "Γλυπτό", HistoricalPeriod = "Ελληνιστική Περίοδος",
                    Status = ItemStatus.Draft, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(328, 0, 0, "cm"),
                    Coordinates = new Coordinates(40.4508, 25.5322),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο Παριανό" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Σαμοθράκη" }
                    }
                },
                new CulturalItem
                {
                    Title = "Καρυάτιδες του Ερεχθείου",
                    Description = "Έξι γυναικείες μορφές που λειτουργούν ως αρχιτεκτονικά στηρίγματα στη νότια στοά του Ερεχθείου.",
                    Category = "Αρχιτεκτονικό Μέλος", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser2.Id,
                    Dimensions = new Dimensions(231, 0, 0, "cm"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο Πεντελικό" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Κλασική Τεχνοτροπία" },
                        new ItemMetadata { Key = "Tag", Value = "Αρχιτεκτονική" }
                    }
                },

                // ── ΕΡΓΑ ΤΕΧΝΗΣ ─────────────────────────────────────────────────
                new CulturalItem
                {
                    Title = "Τοιχογραφία των Ταύρων",
                    Description = "Νωπογραφία από το ανάκτορο της Κνωσού, απεικονίζει την τελετουργική ταυροκαθαψία.",
                    Category = "Έργο Τέχνης", HistoricalPeriod = "Μινωική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser2.Id,
                    Dimensions = new Dimensions(85, 200, 0, "cm"),
                    Coordinates = new Coordinates(35.2983, 25.1631),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Ασβεστοκονίαμα" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Ανάκτορο Κνωσού, Κρήτη" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Νωπογραφία (Fresco)" },
                        new ItemMetadata { Key = "Tag", Value = "Τοιχογραφία" }
                    }
                },
                new CulturalItem
                {
                    Title = "Χρυσό Στεφάνι της Βεργίνας",
                    Description = "Βασιλικό χρυσό στεφάνι δρυός, εύρημα από τον τάφο του Φιλίππου Β' στη Βεργίνα.",
                    Category = "Έργο Τέχνης", HistoricalPeriod = "Ελληνιστική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(0, 0, 0, "N/A"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Χρυσός" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Βεργίνα, Ημαθία" },
                        new ItemMetadata { Key = "Δημιουργός/Καλλιτέχνης", Value = "Άγνωστος Μακεδόνας τεχνίτης" },
                        new ItemMetadata { Key = "Tag", Value = "Κόσμημα" },
                        new ItemMetadata { Key = "Tag", Value = "Βασιλικός Τάφος" }
                    }
                },
                new CulturalItem
                {
                    Title = "Προσωπείο του Αγαμέμνονα",
                    Description = "Χρυσό νεκρικό προσωπείο από τους θολωτούς τάφους των Μυκηνών.",
                    Category = "Έργο Τέχνης", HistoricalPeriod = "Μυκηναϊκή Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser2.Id,
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Χρυσός" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Μυκήνες" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Σφυρήλατη Τεχνική" },
                        new ItemMetadata { Key = "Tag", Value = "Ταφικά Ευρήματα" }
                    }
                },

                // ── ΙΣΤΟΡΙΚΑ ΕΓΓΡΑΦΑ ────────────────────────────────────────────
                new CulturalItem
                {
                    Title = "Δίσκος της Φαιστού",
                    Description = "Πήλινος δίσκος με σπειροειδή ιερογλυφική γραφή, ένα από τα μεγαλύτερα αινίγματα της αρχαιολογίας.",
                    Category = "Ιστορικό Έγγραφο", HistoricalPeriod = "Μινωική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(0, 16, 1, "cm"),
                    Coordinates = new Coordinates(35.0367, 24.8133),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Πηλός" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Ανάκτορο Φαιστού, Κρήτη" },
                        new ItemMetadata { Key = "Tag", Value = "Γραφή" },
                        new ItemMetadata { Key = "Tag", Value = "Άλυτο Αίνιγμα" }
                    }
                },
                new CulturalItem
                {
                    Title = "Ψήφισμα του Θεμιστοκλή",
                    Description = "Μαρμάρινη επιγραφή με το κείμενο του ψηφίσματος για την εκκένωση της Αθήνας πριν τη ναυμαχία της Σαλαμίνας.",
                    Category = "Ιστορικό Έγγραφο", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.ForReview, CreatedById = contributorUser2.Id,
                    Dimensions = new Dimensions(0, 60, 40, "cm"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Τροιζήνα" },
                        new ItemMetadata { Key = "Tag", Value = "Επιγραφή" },
                        new ItemMetadata { Key = "Tag", Value = "Περσικοί Πόλεμοι" }
                    }
                },
                new CulturalItem
                {
                    Title = "Χρυσόβουλλο Ανδρόνικου Β' Παλαιολόγου",
                    Description = "Βυζαντινό χειρόγραφο έγγραφο με χρυσή σφραγίδα, αφορά προνόμια μονής του Αγίου Όρους.",
                    Category = "Ιστορικό Έγγραφο", HistoricalPeriod = "Βυζαντινή Περίοδος",
                    Status = ItemStatus.Draft, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(0, 30, 45, "cm"),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Περγαμηνή" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Άγιον Όρος" },
                        new ItemMetadata { Key = "Δημιουργός/Καλλιτέχνης", Value = "Αυτοκρατορική Γραμματεία Κωνσταντινούπολης" },
                        new ItemMetadata { Key = "Tag", Value = "Χειρόγραφο" }
                    }
                },

                // ── ΜΝΗΜΕΙΑ ─────────────────────────────────────────────────────
                new CulturalItem
                {
                    Title = "Παρθενώνας",
                    Description = "Ναός αφιερωμένος στη θεά Αθηνά Παρθένο, το κορυφαίο μνημείο της Ακρόπολης των Αθηνών.",
                    Category = "Μνημείο", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = adminUser.Id,
                    Dimensions = new Dimensions(6950, 3080, 1386, "cm"),
                    Coordinates = new Coordinates(37.9715, 23.7267),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο Πεντελικό" },
                        new ItemMetadata { Key = "Δημιουργός/Καλλιτέχνης", Value = "Ικτίνος και Καλλικράτης" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Δωρικός Ρυθμός" },
                        new ItemMetadata { Key = "Tag", Value = "Ναός" },
                        new ItemMetadata { Key = "Tag", Value = "Ακρόπολη" }
                    }
                },
                new CulturalItem
                {
                    Title = "Θέατρο της Επιδαύρου",
                    Description = "Αρχαίο θέατρο φημισμένο για την εξαιρετική ακουστική του, φιλοξενεί ακόμη παραστάσεις.",
                    Category = "Μνημείο", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser.Id,
                    Dimensions = new Dimensions(0, 11800, 0, "cm"),
                    Coordinates = new Coordinates(37.5959, 23.0794),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Ασβεστόλιθος" },
                        new ItemMetadata { Key = "Δημιουργός/Καλλιτέχνης", Value = "Πολύκλειτος ο Νεότερος" },
                        new ItemMetadata { Key = "Tag", Value = "Θέατρο" }
                    }
                },
                new CulturalItem
                {
                    Title = "Μηχανισμός των Αντικυθήρων",
                    Description = "Αρχαίος αναλογικός υπολογιστής για την πρόβλεψη αστρονομικών φαινομένων.",
                    Category = "Όργανο", HistoricalPeriod = "Ελληνιστική Περίοδος",
                    Status = ItemStatus.Published, CreatedById = contributorUser2.Id,
                    Dimensions = new Dimensions(1, 33, 18, "cm"),
                    Coordinates = new Coordinates(35.8869, 23.3086),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Χαλκός" },
                        new ItemMetadata { Key = "Τοποθεσία Εύρεσης", Value = "Ναυάγιο Αντικυθήρων" },
                        new ItemMetadata { Key = "Tag", Value = "Αστρονομία" },
                        new ItemMetadata { Key = "Tag", Value = "Τεχνολογία" }
                    }
                },
                new CulturalItem
                {
                    Title = "Ναός του Ποσειδώνα στο Σούνιο",
                    Description = "Δωρικός ναός στο ακρωτήριο Σούνιο, αφιερωμένος στον θεό της θάλασσας.",
                    Category = "Μνημείο", HistoricalPeriod = "Κλασική Περίοδος",
                    Status = ItemStatus.ForReview, CreatedById = contributorUser2.Id,
                    Dimensions = new Dimensions(3100, 1350, 600, "cm"),
                    Coordinates = new Coordinates(37.6501, 24.0250),
                    Metadata = new List<ItemMetadata>
                    {
                        new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο Αγριλέζας" },
                        new ItemMetadata { Key = "Τεχνοτροπία", Value = "Δωρικός Ρυθμός" },
                        new ItemMetadata { Key = "Tag", Value = "Ναός" }
                    }
                }
            };

            await context.CulturalItems.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }
    }
}