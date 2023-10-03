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


### 已知問題
1. rwd首頁隱藏之按鈕
時間顯示

### 待確認

----20231003 討論事項--------
商品刪除後，訂單還要怎麼算 => 1. 訂單抓商品history
款式他不會動，所以先不考慮款式刪除，訂單顯示的情形
結帳時，如果庫存已經滿了，購物車要跳提示
訂單=> 刪除，怕有人誤下單
首頁 新商品倒敘
網路很慢=>中華電信DNS改google
!!買商品的人，一定要登入!!

原圖上船後 先儲存，但不會使用到
個人帳號管理 把二因子跟下載個人資料移除，加入 結帳時會使用到的欄位，但先不代入

售完後，庫存又增加了，將狀態改為販售中
輪播的圖片 他會再給我

類別列表
批次刪除(後面在做)
列表 當前5筆
訂單 搜尋 封存 維護 之後再來
訂單結帳，同時訂購 問題(十來個人搶)