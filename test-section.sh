curl -v -X POST http://localhost:5002/Admin/HomeSections/Create \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -H 'Cookie: .AspNetCore.Identity.Application='$(grep -oP '\.AspNetCore\.Identity\.Application=.*?;' ~/.playwright-mcp/console-* | head -1 | cut -d'=' -f2 | cut -d';' -f1) \
  --data-urlencode 'SectionType=1' \
  --data-urlencode 'Enabled=true' \
  --data-urlencode 'SortOrder=0' \
  --data-urlencode 'Title=CURL TEST' \
  --data-urlencode 'Subtitle=Görünüyor mu test edelim' \
  --data-urlencode '__RequestVerificationToken=XXX'
