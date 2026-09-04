const el = {
  //App-wide
  appName: "Cultural CMS",
  copyright: `© ${new Date().getFullYear()} Σύστημα Διαχείρισης Πολιτιστικού Περιεχομένου`,

  //Navigation / Sidebar
  nav: {
    home: "Αρχική / Αναζήτηση",
    allItems: "Όλα τα Τεκμήρια (Κατάλογος)",
    myItems: "Τα Τεκμήρια μου",
    createItem: "Προσθήκη Τεκμηρίου",
    userManagement: "Διαχείριση Χρηστών",
    login: "Σύνδεση",
    logout: "Αποσύνδεση",
    signup: "Εγγραφή",
    pending: "Προς Έλεγχο"
  },

  // Shared field labels - used across forms, tables and filters
  field: {
    id: "Κωδικός",
    title: "Τίτλος",
    category: "Κατηγορία",
    description: "Περιγραφή",
    historicalPeriod: "Ιστορική Περίοδος",
    status: "Κατάσταση",
    views: "Προβολές",
    username: "Όνομα χρήστη",
    password: "Κωδικός",
    email: "Email",
    firstname: "Όνομα",
    lastname: "Επώνυμο",
    key: "Ιδιότητα",
    value: "Τιμή",
  },

  // Status Labels
  status: {
    Draft: "Πρόχειρο",
    ForReview: "Προς Έλεγχο",
    Published: "Δημοσιευμένο",
  } as Record<string, string>,

  // Audit actions
  audit: {
    Create: "Δημιουργία",
    Update: "Ενημέρωση",
    Delete: "Διαγραφή",
    StatusChange: "Αλλαγή Κατάστασης",
    title: "Ιστορικό Αλλαγών",
    empty: "Δεν βρέθηκαν καταγραφές",
    metadataAdded: "Προσθήκη",
    metadataRemoved: "Διαγραφή",
  } as Record<string, string>,

  statusTransitions: {
    "Draft->ForReview": "Υποβλήθηκε για έλεγχο",
    "ForReview->Published": "Εγκρίθηκε και δημοσιεύθηκε",
    "ForReview->Draft": "Απορρίφθηκε",
  } as Record<string, string>,

  // Home Page
  home: {
    title: "Καλώς ήρθατε στο Cultural CMS",
    subtitle: "Εξερευνήστε τον κατάλογο των πολιτιστικών μας τεκμηρίων.",
  },

  // Auth
  auth: {
    loginButton: "Σύνδεση",
    loginLoading: "Σύνδεση...",
    signupButton: "Εγγραφή",
    signupLoading: "Εγγραφή...",
    signupTitle: "Εγγραφή Χρήστη",
    signUpHere: "Εγγραφείτε εδώ",
    alreadyHaveAccount: "Έχετε ήδη λογαριασμό; Σύνδεση",
    loginHere: "Σύνδεση",
    noAccountYet: "Δεν έχετε λογαριασμό;",
    loginSuccess: "Επιτυχής σύνδεση",
    loginError: "Η σύνδεση απέτυχε.",
    signupSuccess: "Η εγγραφή ολοκληρώθηκε! Παρακαλώ συνδεθείτε.",
    signupError: "Η εγγραφή απέτυχε. Ελέγξτε τα στοιχεία σας.",
    tokenExpired: "Η συνεδρία σας έληξε. Παρακαλώ συνδεθείτε ξανά.",
    signupConflict: "To όνομα χρήστη ή το email χρησιμοποιείται ήδη."
  },

  // Validation
  validation: {
    required: "Το πεδίο είναι υποχρεωτικό.",
    username: "Το όνομα χρήστη πρέπει να έχει 2 έως 50 χαρακτήρες.",
    email: "Το email πρέπει να είναι έγκυρη διεύθυνση.",
    password: "Ο κωδικός πρέπει να έχει 8 έως 25 χαρακτήρες και να περιέχει τουλάχιστον ένα κεφαλαίο, ένα πεζό, έναν αριθμό και έναν ειδικό χαρακτήρα.",
    firstname: "Το όνομα πρέπει να έχει 2 έως 50 χαρακτήρες.",
    lastname: "Το επώνυμο πρέπει να έχει 2 έως 50 χαρακτήρες.",
    title: "Ο τίτλος πρέπει να έχει 2 έως 200 χαρακτήρες.",
    description: "Η περιγραφή δεν πρέπει να υπερβαίνει τους 2000 χαρακτήρες.",
    category: "Η κατηγορία δεν πρέπει να υπερβαίνει τους 100 χαρακτήρες.",
    historicalPeriod: "Η ιστορική περίοδος δεν πρέπει να υπερβαίνει τους 100 χαρακτήρες.",
    metadataKey: "Η ιδιότητα δεν πρέπει να υπερβαίνει τους 100 χαρακτήρες.",
    metadataValue: "Η τιμή δεν πρέπει να υπερβαίνει τους 500 χαρακτήρες.",
  },

  // Cultural Items
  items: {
    createTitle: "Δημιουργία Νέου Τεκμηρίου",
    editTitle: "Επεξεργασία Τεκμηρίου",
    manageTitle: "Διαχείριση Πολιτιστικών Τεκμηρίων",
    myItemsTitle: "Τα Τεκμήρια μου",
    viewDetails: "Προβολή Λεπτομερειών",
    addNewItem: "Προσθήκη Νέου Τεκμηρίου",
    backToList: "Επιστροφή στη Λίστα",
    edit: "Επεξεργασία",
    delete: "Διαγραφή",
    cancel: "Ακύρωση",
    submitForReview: "Υποβολή για Έλεγχο",
    approve: "Έγκριση",
    reject: "Απόρριψη",
    tagLabel: "Ετικέτα",

    // Form sections
    basicInfo: "Βασικές Πληροφορίες",
    photo: "Φωτογραφία",
    photoOptional: "Φωτογραφία (Προαιρετικό)",
    metadata: "Πρόσθετες Ιδιότητες",
    metadataOptional: "Πρόσθετες Ιδιότητες (Προαιρετικό)",
    metadataHint: 'Εισάγετε ιδιότητες (π.χ. Ιδιότητα: "Υλικό", Τιμή: "Μάρμαρο").',
    addMetadata: "+ Προσθήκη",
    removeMetadata: "Αφαίρεση",
    selectFile: "Επιλογή Αρχείου",
    changePhoto: "Αλλαγή Φωτογραφίας",
    preview: "Προεπισκόπηση",

    // Save buttons
    saving: "Αποθήκευση...",
    saveDraft: "Αποθήκευση (Πρόχειρο)",
    updateItem: "Ενημέρωση Τεκμηρίου",

    // Toast messages
    createSuccess: "Το τεκμήριο δημιουργήθηκε επιτυχώς ως Πρόχειρο!",
    createError: "Σφάλμα κατά τη δημιουργία. Ελέγξτε τα στοιχεία.",
    updateSuccess: "Το τεκμήριο ενημερώθηκε επιτυχώς!",
    updateError: "Σφάλμα κατά την ενημέρωση. Ελέγξτε τα στοιχεία.",
    loadError: "Αποτυχία φόρτωσης τεκμηρίου.",
    listError: "Αποτυχία φόρτωσης τεκμηρίων. Ελέγξτε αν τρέχει το backend.",
    myItemsError: "Αποτυχία φόρτωσης των τεκμηρίων σας.",
    deleteConfirm: "Είστε βέβαιοι ότι θέλετε να διαγράψετε αυτό το τεκμήριο; Η ενέργεια δεν αναιρείται.",
    deleteSuccess: "Το τεκμήριο διαγράφηκε επιτυχώς!",
    deleteError: "Αποτυχία διαγραφής. Ελέγξτε τα δικαιώματά σας.",
    submitSuccess: "Υποβλήθηκε επιτυχώς για έλεγχο!",
    submitError: "Η υποβολή απέτυχε. Ελέγξτε τα δικαιώματά σας.",
    approveSuccess: "Το τεκμήριο εγκρίθηκε και δημοσιεύτηκε επιτυχώς!",
    approveError: "Αποτυχία έγκρισης.",
    rejectSuccess: "Το τεκμήριο απορρίφθηκε και επέστρεψε σε κατάσταση Πρόχειρο.",
    rejectError: "Αποτυχία απόρριψης.",
    noItems: "Δεν βρέθηκαν τεκμήρια.",
    noMyItems: "Δεν έχετε δημιουργήσει κανένα τεκμήριο ακόμα.",
  },

  // Search & Filters
  search: {
    label: "Αναζήτηση",
    metaKey: "Ιδιότητα (π.χ. Υλικό)",
    metaValue: "Τιμή (π.χ. Χαλκός)",
    button: "Αναζήτηση",
    advancedFilters: "Προηγμένα Φίλτρα",
    hideFilters: "Απόκρυψη Φίλτρων",
    reset: "Επαναφορά",
    allStatuses: "Όλα",
    sort: {
      by: "Ταξινόμηση κατά",
      order: "Σειρά",
      desc: "Φθίνουσα",
      asc: "Αύξουσα",
      date: "Ημερομηνία",
      title: "Τίτλο",
      popularity: "Δημοφιλία",
    }
  },

  // Roles
  roles: {
    Admin: "Διαχειριστής",
    Curator: "Επιμελητής",
    Contributor: "Δημιουργός"
  },

  // Users
  users: {
    title: "Διαχείριση Χρηστών",
    subtitle: "Αλλαγή ρόλων και δικαιωμάτων πρόσβασης",
    usernameFilter: "Αναζήτηση με Όνομα Χρήστη",
    updateSuccess: "Ο ρόλος ενημερώθηκε επιτυχώς!",
    updateError: "Αποτυχία ενημέρωσης ρόλου.",
    loadError: "Αποτυχία φόρτωσης χρηστών.",
    noUsers: "Δεν βρέθηκαν χρήστες.",
    table: {
      fullName: "Ονοματεπώνυμο",
      role: "Ρόλος",
    },
  },

}

export default el;