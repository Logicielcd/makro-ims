## @core => Models -> menu
app-menu.model.ts -> 
   - ไฟล์สำหรับสร้าง List menu
   - กำหนดชื่อ เมนู ประกอบด้วย MainMenu -> Module -> sub menu name

auth-menu.model.ts -> 
   - PageAction กำหนดว่าทำอะไรได้บ้าง View, Modify, Disable
   - AuthMenu กำหนดชื่อ module และ pageName (sub menu name จาก app-menu)
   - get allMenu() list menu ทั้งหมดที่มี 
   - create static menu variable ตาม list menu ที่มี
      - กำหนด Module, pagename
   - buildMenu function ใช้สำหรับสร้าง menu ตาม roleMenu ของ user
   - buildModuleMenu function ที่เรียกใช้จาก buildMenu ในการสร้าง menu

## @theme => components -> menu
menu.ts ->
   - ใช้สำหรับสร้าง menu label, menu icon, menu link, state (เป็นตัวที่ใช้เช็คกับ user role เทียบเท่า pagename)

menu.component.html ->
   - หน้า display menu

menu.component.ts ->
   - นำข้อมูล current user ดึงจาก authservice.userprofile.getvalue
   - authMenuItem function ใช้สำหรับเช็คว่ามีสิทธิ์เห็นเมนูหรือไม่

ถ้าต้องการเพิ่มหรือลดเมนูให้แก้ไข file ต่อไปนี้
app-menu.model.ts
auth-menu.model.ts
menu.ts

แก้ไฟล์แปลภาษาเมนู
assets->i18n->en.json , th.json
