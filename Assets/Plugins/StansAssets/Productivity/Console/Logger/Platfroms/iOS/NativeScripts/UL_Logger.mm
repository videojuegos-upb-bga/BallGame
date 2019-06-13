////////////////////////////////////////////////////////////////////////////////
//
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////
#if !TARGET_OS_TV
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>



NSString * const UNITY_SPLITTER = @"|";
NSString * const UNITY_EOF = @"endofline";
NSString * const ARRAY_SPLITTER = @"%%%";

@interface DataConvertor : NSObject

@end

@implementation DataConvertor : NSObject


+(NSString *) charToNSString:(char *)value {
    if (value != NULL) {
        return [NSString stringWithUTF8String: value];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}

@end

//////////////////////////////////////////
// LoggerViewController - View controller to select Logs from device to share
//////////////////////////////////////////
@interface LoggerViewController : UITableViewController

@property UIBarButtonItem *doneItem, *editItem, *deleteItem, *shareItem, *backItem;
@property NSDictionary *allLogFilenames;
@property NSArray *globalLogs;
@property NSDateFormatter *cellDateFormatter;
@property NSMutableArray<NSIndexPath*> *selectedIndexPaths;

@property UIBarButtonItem *shareButton, *deleteButton, *flexibleItem, *selectAllButton;
@end

//////////////////////////////////////////
// PreviewLogViewController - View controller to slide between Full and Unity log
//////////////////////////////////////////
@interface PreviewLogViewController : UIPageViewController
@property NSString *globalLogPath;
@property UIBarButtonItem *deleteButton, *shareButton;
@property BOOL isStandalone;
@end

//////////////////////////////////////////
// LogTableViewCell - Cell to display info about log
//////////////////////////////////////////
@interface LogTableViewCell: UITableViewCell
@end

@interface LogTextViewController: UIViewController
@property NSString *filePath;
@property UITextView *textView;
@end

@interface PathWithModDate : NSObject
@property (strong) NSString *path;
@property (strong) NSString *name;
@property (strong) NSDate *modDate;
@end

@implementation PathWithModDate
@end


//////////////////////////////////////////
// UL_Logger - singleton to manage log redirection and sharing
//////////////////////////////////////////
@interface UL_Logger : NSObject <UIPageViewControllerDataSource, UIPageViewControllerDelegate> {}
+(instancetype)sharedInstance;

- (void)redirectLogToFile;
- (UINavigationController *)logListViewController;
- (PreviewLogViewController *)logPagerForFiles;
- (NSDictionary*)getLogFileNames;
- (void)logUnityOnly:(NSString*) textToWrite;
- (UINavigationController *)currentSessionLogsViewController;

@property LogTextViewController *globalLogVC;
@property PreviewLogViewController *pagerVC;
@property NSInteger selectedLogIndex;

@property NSString *documentsDirectoryPath;
@property NSString *logDirectoryPath;

@property NSString *logFileName;
@property NSString *logFilePath;

@property NSString *unityLogName;
@property NSString *unityLogPath;

@property NSDateFormatter *dateFormatter;
@property NSString *projectName;
@end


@interface Log : NSObject
void _Log(const char *file, int lineNumber, const char *funcName, NSString *format,...);
@end


@implementation Log
void append(NSString *msg){
    // get path to Documents/somefile.txt
    NSString *path = [UL_Logger sharedInstance].logFilePath;
    // create if needed
    if (![[NSFileManager defaultManager] fileExistsAtPath:path]){
        [[NSData data] writeToFile:path atomically:YES];
    }
    // append
    NSFileHandle *handle = [NSFileHandle fileHandleForWritingAtPath:path];
    [handle truncateFileAtOffset:[handle seekToEndOfFile]];
    [handle writeData:[msg dataUsingEncoding:NSUTF8StringEncoding]];
    [handle closeFile];
}

void _Log(const char *file, int lineNumber, const char *funcName, NSString *format,...) {
    static NSDateFormatter* timeStampFormat;
    if (!timeStampFormat) {
        timeStampFormat = [[NSDateFormatter alloc] init];
        [timeStampFormat setDateFormat:@"yyyy-MM-dd HH:mm:ss.SSS"];
        [timeStampFormat setTimeZone:[NSTimeZone systemTimeZone]];
    }
    NSString* timestamp = [timeStampFormat stringFromDate:[NSDate date]];
    
    va_list ap;
    va_start (ap, format);
    format = [format stringByAppendingString:@"\n"];
    NSString *msg = [[NSString alloc] initWithFormat:[NSString stringWithFormat:@"%@ %@",timestamp, format] arguments:ap];
    va_end (ap);
    fprintf(stderr,"%s", [msg UTF8String]);
    append(msg);
}

@end

@implementation UL_Logger {}
static NSString * const ISN_LOG_DIRECTORY_NAME = @"ISN_Logs";
static NSString * const GLOBAL_LOG_FILENAME_DICTIONARY_KEY = @"globalLogFileNames";

NSString *appName;

+(instancetype)sharedInstance{
    static dispatch_once_t onceToken;
    static UL_Logger *sharedInstance;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[UL_Logger alloc] init];
    });
    return sharedInstance;
}

- (id) init {
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    _documentsDirectoryPath = [paths objectAtIndex:0];
    _logDirectoryPath = [_documentsDirectoryPath stringByAppendingPathComponent:ISN_LOG_DIRECTORY_NAME];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:_logDirectoryPath isDirectory:nil]) {
        NSError *error;
        [[NSFileManager defaultManager] createDirectoryAtPath:_logDirectoryPath withIntermediateDirectories:true attributes:nil error:&error];
    }
    
    _dateFormatter = [[NSDateFormatter alloc] init];
    [_dateFormatter setDateFormat:@"MMM dd hh:mm"];
    NSString *currentDate = [_dateFormatter stringFromDate:[NSDate date]];
    
    
    NSDictionary *bundleInfo = [[[NSBundle mainBundle] infoDictionary] init];
    NSString *bundleAppName = [bundleInfo objectForKey:@"CFBundleDisplayName"];
    appName = [bundleAppName stringByAppendingString:@" - "];
    _logFileName = [NSString stringWithFormat:@"%@%@.txt", appName, currentDate];
    
    _logFilePath = [_logDirectoryPath stringByAppendingPathComponent:_logFileName];
    
    return self;
}

- (void)redirectLogToFile {
    [[NSFileManager defaultManager] createFileAtPath:self.logFilePath contents:[NSData data] attributes:nil];
    [[NSFileManager defaultManager] createFileAtPath:self.unityLogPath contents:[NSData data] attributes:nil];
    if (!isatty(STDERR_FILENO)) {
        freopen([self.logFilePath cStringUsingEncoding:NSASCIIStringEncoding],"a+",stderr);
    } else {
#define NSLog(args...) _Log(__FILE__,__LINE__,__PRETTY_FUNCTION__,args);
    }
    //
}

- (UINavigationController *)logListViewController {
    LoggerViewController *ctrl = [[LoggerViewController alloc] init];
    UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:ctrl];
    return navigationController;
}

- (PreviewLogViewController *)logPagerForFiles {
    self.pagerVC = [[PreviewLogViewController alloc]initWithTransitionStyle:UIPageViewControllerTransitionStyleScroll navigationOrientation:UIPageViewControllerNavigationOrientationHorizontal options:nil];
    self.pagerVC.isStandalone = false;
    _globalLogVC = [[LogTextViewController alloc]init];
    
    NSDictionary *_allLogFilenames = [self getLogFileNames];
    NSArray *globalLogs = [_allLogFilenames objectForKey:GLOBAL_LOG_FILENAME_DICTIONARY_KEY];
    
    
    if (globalLogs.count && globalLogs.count > _selectedLogIndex) {
        _globalLogVC.filePath = [self.logDirectoryPath stringByAppendingPathComponent:[globalLogs objectAtIndex:(NSUInteger)_selectedLogIndex]];
    }
    
    self.pagerVC.globalLogPath = _globalLogVC.filePath;
    
    [self.pagerVC setDelegate:self];
    
    [self.pagerVC setViewControllers:[NSArray arrayWithObjects:_globalLogVC, nil] direction:UIPageViewControllerNavigationDirectionForward animated:true completion:^(BOOL finished) {
        
    }];
    
    return self.pagerVC;
}

- (PreviewLogViewController *)logPagerForCurrentSession {
    self.pagerVC = [[PreviewLogViewController alloc]initWithTransitionStyle:UIPageViewControllerTransitionStyleScroll navigationOrientation:UIPageViewControllerNavigationOrientationHorizontal options:nil];
    self.pagerVC.isStandalone = true;
    _globalLogVC = [[LogTextViewController alloc]init];
    
    _globalLogVC.filePath = self.logFilePath;
    
    self.pagerVC.globalLogPath = self.logFilePath;
    
    [self.pagerVC setDelegate:self];
    
    [self.pagerVC setViewControllers:[NSArray arrayWithObjects:_globalLogVC, nil] direction:UIPageViewControllerNavigationDirectionForward animated:true completion:^(BOOL finished) {
        
    }];
    
    return self.pagerVC;
}

- (void)logUnityOnly:(NSString*) textToWrite {
    NSString *currentTimeString = [self.dateFormatter stringFromDate:[NSDate date]];
    NSString *outputString = [NSString stringWithFormat:@"%@: %@", currentTimeString, textToWrite];
    
    if ([[NSFileManager defaultManager] fileExistsAtPath:self.unityLogPath]) {
        NSFileHandle *fileHandle = [NSFileHandle fileHandleForWritingAtPath:self.unityLogPath];
        [fileHandle seekToEndOfFile];
        [fileHandle writeData:[outputString dataUsingEncoding:NSUTF8StringEncoding]];
        [fileHandle closeFile];
    }
    NSLog(@"%@", textToWrite); // Logging to global log unity messages too
}

- (NSDictionary*)getLogFileNames {
    
    NSMutableArray *globalLogFileNames = [NSMutableArray new];
    NSMutableDictionary *logFiles = [NSMutableDictionary new];
    
    NSArray *sortedFiles = [self getFilesAtPathSortedByModificationDate:_logDirectoryPath];
   
    for (PathWithModDate* fileInfo in sortedFiles) {
        
        //shoudl be more like matches a patern
        if ([fileInfo.name containsString:appName]) {
            [globalLogFileNames addObject:fileInfo.name];
        }
    }
    
    [logFiles setObject:globalLogFileNames forKey:GLOBAL_LOG_FILENAME_DICTIONARY_KEY];
    return logFiles;
}


- (NSArray*)getFilesAtPathSortedByModificationDate:(NSString*)folderPath {
    NSArray *allPaths = [NSFileManager.defaultManager contentsOfDirectoryAtPath:folderPath error:nil];
    
    NSMutableArray *sortedPaths = [NSMutableArray new];
    for (NSString *fileName in allPaths) {
        NSString *fullPath = [folderPath stringByAppendingPathComponent:fileName];
        
        NSDictionary *attr = [NSFileManager.defaultManager attributesOfItemAtPath:fullPath error:nil];
        NSDate *modDate = [attr objectForKey:NSFileCreationDate]; //NSFileModificationDate
        
        PathWithModDate *pathWithDate = [[PathWithModDate alloc] init];
        pathWithDate.name = fileName;
        pathWithDate.path = fullPath;
        pathWithDate.modDate = modDate;
       
        [sortedPaths addObject:pathWithDate];
    }
    
    [sortedPaths sortUsingComparator:^(PathWithModDate *path1, PathWithModDate *path2) {
        // Descending (most recently modified first)
        return [path2.modDate compare:path1.modDate];
    }];
    
    return sortedPaths;
}

- (UIViewController *)pageViewController:(UIPageViewController *)pageViewController viewControllerBeforeViewController:(UIViewController *)viewController {
    return _globalLogVC;
}

- (UIViewController *)pageViewController:(UIPageViewController *)pageViewController viewControllerAfterViewController:(UIViewController *)viewController {
    return _globalLogVC;
}

- (UINavigationController *)currentSessionLogsViewController {
    [UL_Logger sharedInstance].selectedLogIndex = 0;
    
    PreviewLogViewController *pagerVC = [[UL_Logger sharedInstance]logPagerForCurrentSession];
    pagerVC.isStandalone = true;
    UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:pagerVC];
    return navigationController;
}

@end

@implementation LogTableViewCell

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier {
    self = [super initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:reuseIdentifier];
    self.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    return self;
}

@end

@implementation LoggerViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    [self adjustToolbar];
    
    _selectedIndexPaths = [NSMutableArray<NSIndexPath*> new];
    
    _allLogFilenames = [[UL_Logger sharedInstance] getLogFileNames];
    _globalLogs = [_allLogFilenames objectForKey:GLOBAL_LOG_FILENAME_DICTIONARY_KEY];
    
    
    [self.tableView registerClass:[LogTableViewCell class] forCellReuseIdentifier:@"fileCell"];
    [self.tableView setAllowsMultipleSelectionDuringEditing:true];
    
    [self.navigationItem setTitle:@"Logs"];
    
    _editItem = [[UIBarButtonItem alloc ]initWithTitle:@"Select" style:UIBarButtonItemStylePlain target:self action:@selector(setEditMode:)];
    _doneItem = [[UIBarButtonItem alloc ]initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(setNormalMode:)];
    _backItem = [[UIBarButtonItem alloc ]initWithTitle:@"Close" style:UIBarButtonItemStylePlain target:self action:@selector(closeAction)];
    
    [self.navigationItem setRightBarButtonItem:_editItem];
    [self.navigationItem setLeftBarButtonItem:_backItem];
}

- (void) viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [self.navigationController setToolbarHidden:true];
}

- (void) viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    if (self.tableView) {
        _allLogFilenames = [[UL_Logger sharedInstance] getLogFileNames];
        _globalLogs = [_allLogFilenames objectForKey:GLOBAL_LOG_FILENAME_DICTIONARY_KEY];
        
        [self.tableView reloadData];
    }
}

- (void)setEditMode:(id)button {
    [self.tableView setEditing:true animated:true];
    [self.navigationItem setLeftBarButtonItem:nil];
    [self.navigationItem setRightBarButtonItem:_doneItem];
    [_selectAllButton setTitle:@"Select All"];
    [self setToolbarItems:[NSArray arrayWithObjects:_deleteButton, _flexibleItem, _selectAllButton, _flexibleItem, _shareButton, nil]];
    [self.navigationController setToolbarHidden:false animated:true];
}

- (void)setNormalMode:(id)button {
    [_selectedIndexPaths removeAllObjects];
    [self.tableView setEditing:false animated:true];
    [self.navigationItem setLeftBarButtonItem:_backItem];
    [self.navigationItem setRightBarButtonItem:_editItem];
    [self setToolbarItems:[NSArray arrayWithObjects:_flexibleItem, _shareButton, nil]];
    [self.navigationController setToolbarHidden:true animated:true];
}

- (void)closeAction {
    [self.navigationController dismissViewControllerAnimated:true completion:^{
        
    }];
}

- (void)selectAllAction {
    
    if ([_selectAllButton.title isEqualToString:@"Select All"]) {
        for (int i = 0; i < [self.tableView numberOfSections]; i++) {
            for (int j = 0; j < [self.tableView numberOfRowsInSection:i]; j++) {
                NSIndexPath *indexPath = [NSIndexPath indexPathForRow:(NSInteger)j inSection:(NSInteger)i];
                if (![_selectedIndexPaths containsObject:indexPath]) {
                    [_selectedIndexPaths addObject:indexPath];
                }
                [self.tableView selectRowAtIndexPath:indexPath animated:false scrollPosition:UITableViewScrollPositionNone];
            }
        }
        [_selectAllButton setTitle:@"Deselect All"];
    } else {
        for (int i = 0; i < [self.tableView numberOfSections]; i++) {
            for (int j = 0; j < [self.tableView numberOfRowsInSection:i]; j++) {
                NSIndexPath *indexPath = [NSIndexPath indexPathForRow:(NSInteger)j inSection:(NSInteger)i];
                [_selectedIndexPaths removeObject:indexPath];
                [self.tableView deselectRowAtIndexPath:indexPath animated:false];
            }
        }
        [_selectAllButton setTitle:@"Select All"];
    }
    
}

- (void)shareAction:(id)sender {
    
    NSArray<NSIndexPath *> *indexPaths = _selectedIndexPaths;
    
    if (indexPaths.count > 0) {
        NSMutableArray *itemsToShare = [NSMutableArray arrayWithObject:@"Sharing Logs"];
        NSString *logDirectoryPath = [[UL_Logger sharedInstance] logDirectoryPath];
        
        for (NSIndexPath* indexPath in indexPaths) {
            
            NSString *globalLogfilePath = [logDirectoryPath stringByAppendingPathComponent:[_globalLogs objectAtIndex:(NSUInteger) indexPath.row]];
            
            if ([[NSFileManager defaultManager] fileExistsAtPath:globalLogfilePath]) {
                NSURL *url = [NSURL fileURLWithPath:globalLogfilePath];
                [itemsToShare addObject:url];
            }
        }
        
        UIActivityViewController *activityViewController = [[UIActivityViewController alloc]initWithActivityItems:itemsToShare applicationActivities:nil];
        
        activityViewController.popoverPresentationController.barButtonItem = self.shareButton;
        
        activityViewController.excludedActivityTypes = @[UIActivityTypePostToVimeo,
                                                         UIActivityTypePostToTwitter,
                                                         UIActivityTypePostToWeibo,
                                                         UIActivityTypePostToFacebook
                                                         ];
        
        [self.navigationController presentViewController:activityViewController animated:true completion:^{
            
        }];
    } else {
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"Nothing to share" message:@"First, select logs to share" preferredStyle:UIAlertControllerStyleAlert];
        
        
        UIAlertAction* okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:nil];
        [alertController addAction:okAction];
        alertController.popoverPresentationController.barButtonItem = self.shareButton;
        [self.navigationController presentViewController:alertController animated:YES completion:^{
            [self setEditMode:nil];
        }];
        
    }
    
    
}

- (void)deleteAction:(id)sender {
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"" message:@"" preferredStyle:UIAlertControllerStyleAlert];
    alertController.popoverPresentationController.barButtonItem = self.deleteButton;
    if (_selectedIndexPaths.count > 0) {
        [alertController setMessage:@"Confirm removing selected logs"];
        UIAlertAction* yesAction = [UIAlertAction actionWithTitle:@"YES" style:UIAlertActionStyleDestructive handler:^(UIAlertAction *action)
                                    {
                                        [self deleteLogs];
                                    }];
        UIAlertAction* noAction = [UIAlertAction actionWithTitle:@"NO" style:UIAlertActionStyleCancel handler:nil];
        
        [alertController addAction:yesAction];
        [alertController addAction:noAction];
    } else {
        [alertController setTitle:@"Nothing to remove"];
        [alertController setMessage:@"First, select logs to remove"];
        UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleCancel handler:nil];
        [alertController addAction:okAction];
    }
    
    
    [self presentViewController:alertController animated:YES completion:^{
        
    }];
    
}

- (void)cancelAction:(id)sender {
    
    [[self presentingViewController] dismissViewControllerAnimated:true completion:^{
        //        NSLog(@"user has just canceled sharing action");
    }];
}

- (void)adjustToolbar {
    
    _shareButton = [[UIBarButtonItem alloc]
                    initWithBarButtonSystemItem:UIBarButtonSystemItemAction
                    target:self
                    action:@selector(shareAction:)];
    
    _deleteButton = [[UIBarButtonItem alloc]
                     initWithBarButtonSystemItem:UIBarButtonSystemItemTrash
                     target:self
                     action:@selector(deleteAction:)];
    
    _flexibleItem = [[UIBarButtonItem alloc]
                     initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace
                     target:nil
                     action:nil];
    
    _selectAllButton = [[UIBarButtonItem alloc]
                        initWithTitle:@"Select All" style:UIBarButtonItemStylePlain
                        target:self
                        action:@selector(selectAllAction)];
    
    [self setToolbarItems:[NSArray arrayWithObjects:_flexibleItem, _shareButton, nil]];
    
    
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    
    if (_globalLogs) {
        return _globalLogs.count;
    }
    
    return 0;
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"fileCell"];
    
    NSString *fileName;
    
    if (_globalLogs && _globalLogs.count > indexPath.row) {
        fileName = [[_globalLogs objectAtIndex:(NSUInteger)indexPath.row] mutableCopy];
    }
    
    NSString *cellText;
    if (fileName) {
        cellText = [[fileName stringByReplacingOccurrencesOfString:appName withString:@""] mutableCopy];
        cellText = [[cellText stringByReplacingOccurrencesOfString:@".txt" withString:@""] mutableCopy];
    }
    
    NSDateFormatter *loggerDateFormatter = [UL_Logger sharedInstance].dateFormatter;
    NSDate *date = [loggerDateFormatter dateFromString:cellText];
    
    if (_cellDateFormatter == nil) {
        _cellDateFormatter = [[NSDateFormatter alloc] init];
        [_cellDateFormatter setDateFormat:@"dd MMM – h:mm"];
    }
    
    NSString *readableDateTime = [_cellDateFormatter stringFromDate:date];
    
    NSString *filePath = [[UL_Logger sharedInstance].logDirectoryPath stringByAppendingPathComponent:fileName];
    double fileSize = (double) [[[NSFileManager defaultManager] attributesOfItemAtPath:filePath error:nil] fileSize];
    
    NSString *fileSizeString = @"0 KB";
    
    if (fileSize < 1024) {
        fileSizeString = [NSString stringWithFormat:@" %3.0f B", fileSize];
    } else if (fileSize < 1024*1024) {
        fileSizeString = [NSString stringWithFormat:@"%6.3f KB", fileSize / 1024.0];
    } else {
        fileSizeString = [NSString stringWithFormat:@"%6.3f MB", fileSize / 1024.0 / 1024.0];
    }
    
    [cell.textLabel setText:readableDateTime];
    [cell.detailTextLabel setText: fileSizeString];
    
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    return true;
}

- (BOOL)tableView:(UITableView *)tableView shouldIndentWhileEditingRowAtIndexPath:(NSIndexPath *)indexPath
{
    return true;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    if (tableView.isEditing) {
        [_selectedIndexPaths addObject:indexPath];
    } else {
        
        [UL_Logger sharedInstance].selectedLogIndex = indexPath.row;
        
        PreviewLogViewController *pagerVC = [[UL_Logger sharedInstance]logPagerForFiles];
        
        [self.navigationController pushViewController:pagerVC animated:true];
    }
    
}

- (void)tableView:(UITableView *)tableView didDeselectRowAtIndexPath:(NSIndexPath *)indexPath {
    [_selectedIndexPaths removeObject:indexPath];
}

- (void)deleteLogs {
    NSString *logDirectoryPath = [[UL_Logger sharedInstance] logDirectoryPath];
    
    NSString *globalLogfilePath;
    
    if (_selectedIndexPaths.count > 0) {
        for (NSIndexPath *indexPath in _selectedIndexPaths) {
            globalLogfilePath = [logDirectoryPath stringByAppendingPathComponent:[_globalLogs objectAtIndex:(NSUInteger) indexPath.row]];
            NSError *error;
            [[NSFileManager defaultManager] removeItemAtPath:globalLogfilePath error:&error];
        }
    } else {
        for (int j = 0; j < (int)_globalLogs.count; j++) {
            globalLogfilePath = [logDirectoryPath stringByAppendingPathComponent:[_globalLogs objectAtIndex:(NSUInteger) j]];
            NSError *error;
            [[NSFileManager defaultManager] removeItemAtPath:globalLogfilePath error:&error];
        }
    }
    
    [_selectedIndexPaths removeAllObjects];
    
    _allLogFilenames = [[UL_Logger sharedInstance] getLogFileNames];
    
    if (_allLogFilenames) {
        _globalLogs = [_allLogFilenames objectForKey:GLOBAL_LOG_FILENAME_DICTIONARY_KEY];
    }
    [self.tableView reloadData];
    
}

@end

@implementation PreviewLogViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    [self.view setBackgroundColor:[UIColor grayColor]];
    [self setAutomaticallyAdjustsScrollViewInsets:false];
    
    _deleteButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemTrash target:self action:@selector(deleteAction)];
    _shareButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemAction target:self action:@selector(shareAction)];
    [self.navigationItem setRightBarButtonItem:_shareButton];
    
    if (_isStandalone) {
        UIBarButtonItem *dismissButton = [[UIBarButtonItem alloc] initWithTitle:@"Close" style:UIBarButtonItemStylePlain target:self action:@selector(dismissAction)];
        [self.navigationItem setLeftBarButtonItem:dismissButton];
    }
    
    [self.navigationItem setTitle:@"Log Viewer"];
}

- (void)deleteAction {
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"" message:@"Confirm removing current logs" preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction* yesAction = [UIAlertAction actionWithTitle:@"YES" style:UIAlertActionStyleDestructive handler:^(UIAlertAction *action)
                                {
                                    [[NSFileManager defaultManager]removeItemAtPath:_globalLogPath error:nil];
                                    if (!_isStandalone) {
                                        [self.navigationController popViewControllerAnimated:true];
                                    } else {
                                        [self.navigationController dismissViewControllerAnimated:true completion:^{}];
                                    }
                                }];
    UIAlertAction* noAction = [UIAlertAction actionWithTitle:@"NO" style:UIAlertActionStyleCancel handler:nil];
    
    alertController.popoverPresentationController.barButtonItem = self.deleteButton;
    [alertController addAction:yesAction];
    [alertController addAction:noAction];
    
    [self presentViewController:alertController animated:true completion:^{}];
}

- (void)dismissAction {
    [self.navigationController dismissViewControllerAnimated:true completion:^{}];
}

-(void)shareAction {
    
    NSMutableArray *itemsToShare = [NSMutableArray arrayWithObject:@"Attached Files"];
    
    if ([[NSFileManager defaultManager] fileExistsAtPath:_globalLogPath]) {
        NSURL *url = [NSURL fileURLWithPath:_globalLogPath];
        [itemsToShare addObject:url];
    }
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc]initWithActivityItems:itemsToShare applicationActivities:nil];
    activityViewController.excludedActivityTypes = @[UIActivityTypePostToVimeo,
                                                     UIActivityTypePostToTwitter,
                                                     UIActivityTypePostToWeibo,
                                                     UIActivityTypePostToFacebook
                                                     ];
    
    activityViewController.popoverPresentationController.barButtonItem = self.shareButton;
    [self.navigationController presentViewController:activityViewController animated:true completion:^{
        
    }];
}

@end

@implementation LogTextViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    UITextView *textView = [[UITextView alloc]init];
    
    [self.view addSubview:textView];
    [textView setBackgroundColor:[UIColor whiteColor]];
    [textView setTextColor:[UIColor darkTextColor]];
    [textView setEditable:false];
    textView.translatesAutoresizingMaskIntoConstraints = false;
    
    [self.view addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[textView]|" options:0 metrics:nil views:
                               _NSDictionaryOfVariableBindings(@"textView", textView)]];
    [self.view addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[topLayoutGuide]-0-[textView]-0-[bottomLayoutGuide]|" options:0 metrics:nil views:@{@"textView": textView, @"topLayoutGuide":self.topLayoutGuide, @"bottomLayoutGuide":self.bottomLayoutGuide}]];
    [self.view setNeedsUpdateConstraints];
    
    
    if (_filePath && [[NSFileManager defaultManager] fileExistsAtPath:_filePath]) {
        NSString *content = [NSString stringWithContentsOfFile:_filePath encoding:NSUTF8StringEncoding error:nil];
        [textView setText:content];
    } else {
        [textView setText:@"file not found"];
    }
}

@end



extern "C" {
    
    
    //--------------------------------------
    // Logger Implementation
    //--------------------------------------
    
    void _UL_Init() {
        NSLog(@"UL_Logger: init");
        [[UL_Logger sharedInstance] redirectLogToFile];
    }
    
    void _UL_ShowSharingUI() {
        UIViewController *ctrl = [[UL_Logger sharedInstance] logListViewController];
        
        UIViewController *vc =  UnityGetGLViewController();
        [vc presentViewController:ctrl animated:YES completion:nil];
    }
    
    void _UL_ShowSessionLog() {
        UINavigationController *ctrl = [[UL_Logger sharedInstance] currentSessionLogsViewController];
        
        UIViewController *vc =  UnityGetGLViewController();
        [vc presentViewController:ctrl animated:YES completion:nil];
    }
    
    void _UL_LogMessage(char* type, char* message) {
        NSString *tag = [DataConvertor charToNSString:type];
        NSString *logMessage = [DataConvertor charToNSString:message];
        
        NSString *textToWrite = [NSString stringWithFormat:@"[%@]: %@", tag, logMessage];
        
        [[UL_Logger sharedInstance] logUnityOnly:textToWrite];
    }
    
    char* _UL_GetSessionLog() {
        NSString *pathToCurrentLog = [UL_Logger sharedInstance].logFilePath;
        NSString *log;
        if ([[NSFileManager defaultManager] fileExistsAtPath:pathToCurrentLog]) {
            log = [NSString stringWithContentsOfFile:pathToCurrentLog encoding:NSUTF8StringEncoding error:nil];
        } else {
            log = @"Current session log files not found";
        }
        
        const char* string = [log UTF8String];
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
}

#endif
