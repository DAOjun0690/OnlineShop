Online Shop 

建立資料夾 AppData/Database
local端建立空檔案 AppData/Database/sqlite.db

Add-Migration

---
### 待實現
- UI 兔子化
- 解釋跟促銷 套 ckeditor
- 分類頁表 layout 為了編輯分類
- viewdata["title"] 改用 onlineShop 
- 商品增加 建立日期 欄位
- 網站更新時，放 自訂的 403圖片 在 app service上
- 購物車，目前是使用seesion，之後可能要儲存在db當中 
- 商品編輯 儲存的時候 看有沒有更好的方法
- 輪播圖片，看能不能換地方，目的是換輪播圖片，可以不用重新過版

### 已知問題
- rwd首頁隱藏之按鈕
- 時間顯示
- ckeditor 上傳圖片 要可以調大小

### 待確認
- 使用者帳號有沒有要修的
- 購物車 返回上一頁操作 下拉選單 會變空白

#### 20231003 討論事項
- 批次刪除(後面在做)
- 訂單 搜尋 封存 維護 之後再來
- 訂單結帳，同時訂購 問題(十來個人搶)

#### 第一期學費 393

#### 20231028 功能與需求
- 圖片(△)跟js要用cache 
- 超商api 使用者自己倒出，或是ifrmae，像是: https://emap.pcsc.com.tw/ecmap/default.aspx
- 線上商店，可以用商品類別 進行篩選，類別以下拉選單呈現，就像兔子網站依樣
##### 神秘問題
5吋手機 神秘問題 跑馬燈正常，但是商品圖片沒有出來
已知有問題手機型號
iphone 15
HTC 
Oppo Reno6 pro

<iframe id="map-iframe" src="https://www.family.com.tw/Marketing/storemap/?v=1" style="min-height: 0px; height: 756px;"></iframe>
