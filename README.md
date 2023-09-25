Online Shop 

Add-Migration

### 待實現
UI 兔子化
解釋跟促銷 套 ckeditor
分類頁表 layout 為了編輯分類
viewdata["title"] 改用 onlineShop 
編輯的詳細頁面 與商品的詳細頁面 可以用同一頁，購買按鈕隱藏這樣
購買時 後端數量要再作驗證
購買時 前端最小設為0
RevieceOrder 跟 Order/Detials 共用同一頁

輪播的三張圖片，要可以設定
azure db備份
azure log
azure storage(圖片)


### 已知問題
1. rwd首頁隱藏之按鈕
詳細頁面 Content致中
下訂單 的返回購物車事件 目前無反應

### 待確認
款式與價錢綁再一起 要確認
購買者 允許null?
加入購物車 那庫存要鎖起來嗎
圖片太大
款式只有一種，購買時還要出現嗎
帳號需要先開放嗎?> 我想先所以起來，會影響購買者要帶入的資料跟填寫的東西
購買的非同步 => 已售完 但是另一個人正好下訂單
下單後 商品進行更改->要保留購買當下 商品的狀態?

// robert
款式藏到名稱下