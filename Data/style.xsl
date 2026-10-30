<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template match="/">
    <html>
      <head>
        <meta charset="utf-8" />
        <title>Каталог DMarket</title>
      </head>
      <body>
        <h2>Каталог предметів маркетплейсу DMarket</h2>
        <table border="1" cellpadding="5" cellspacing="0">
          <tr>
            <th>ID</th>
            <th>Назва предмета</th>
            <th>Гра</th>
            <th>Категорія</th>
            <th>Рідкість</th>
            <th>Зношеність / Герой</th>
            <th>Ціна ($)</th>
            <th>Продавець</th>
            <th>Теги</th>
          </tr>
          <xsl:for-each select="DMarketCatalog/Item">
            <tr>
              <td><xsl:value-of select="@Id" /></td>
              <td><xsl:value-of select="@Title" /></td>
              <td><xsl:value-of select="@Game" /></td>
              <td><xsl:value-of select="@Category" /></td>
              <td><xsl:value-of select="@Rarity" /></td>
              <td>
                <xsl:value-of select="@Exterior" />
                <xsl:value-of select="@Hero" />
              </td>
              <td><xsl:value-of select="@Price" /></td>
              <td><xsl:value-of select="@Seller" /></td>
              <td>
                <xsl:for-each select="Tag">
                  <xsl:value-of select="@Name" />
                  <xsl:if test="position() != last()">, </xsl:if>
                </xsl:for-each>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>