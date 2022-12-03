import os
import openpyxl
wb = openpyxl.load_workbook("怪物信息表.xlsx")
sh = wb['Sheet1']
file_names = os.listdir('./')
row = 2
for name in file_names:
  if(name.endswith('.png')):
    sh.cell(row, 2).value = name
    row += 1
wb.save("怪物信息表.xlsx")
    