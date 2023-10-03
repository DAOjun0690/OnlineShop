Online Shop 

建立資料夾 AppData/Database
local端建立空檔案 AppData/Database/sqlite.db

Add-Migration

---

### 待實現
UI 兔子化
解釋跟促銷 套 ckeditor
分類頁表 layout 為了編輯分類
viewdata["title"] 改用 onlineShop 

輪播的三張圖片，要可以設定
azure db備份
azure log
D:\home\site\wwwroot\AppData
\\mounts\\AppData

訂單列表的Pagger功能
如果sold out ，加入購物車頁面，應該要改成售完

### 已知問題
1. rwd首頁隱藏之按鈕
時間顯示
訂單列表 金額顯示錯誤

### 待確認
購買者 允許null?
加入購物車 那庫存要鎖起來嗎
帳號需要先開放嗎?> 我想先所以起來，會影響購買者要帶入的資料跟填寫的東西
購買的非同步 => 已售完 但是另一個人正好下訂單
下單後 商品進行更改->要保留購買當下 商品的狀態?
原圖上船後 需要儲存嗎
商品刪除後，訂單還要怎麼算

