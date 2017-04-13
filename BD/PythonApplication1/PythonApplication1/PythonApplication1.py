import pypyodbc
import json

ConnectionString = r"DRIVER={ODBC Driver 13 for SQL Server};SERVER=DESKTOP-H3IDCPD\SQLEXPRESS;Database=Magic;Trusted_Connection=yes"

conn = pypyodbc.connect(ConnectionString)
c = conn.cursor()

SQL = 'SELECT * FROM <YOURTABLE>'

c.execute('USE Magic')
c.execute("SELECT * FROM [User]")

f = open(r"C:\Users\David\Desktop\AllSets.json", "r", encoding="utf8")

magic_data = json.loads(f.read())

f.close()

for f in magic_data:
    set = magic_data[f]
    print(set['name'])

