<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">
		<html>
			<head>
				<!-- <link href="tableStyle.css" type="text/css" rel="stylesheet" /> -->
				<style>
					table.doc {
						margin: 0;
						padding: 0;
						border-collapse: collapse;
					}
					
					table.doc tr:nth-of-type(odd) {
						background: #eee;
					}
					
					table.doc tr th {
						background: rgb(0, 160, 201);
						color: white;
					}
					
					table.doc td,table.doc th {
						padding: 6px;
						border: 1px solid #ccc;
						text-align: left;
					}	
				</style>
			</head>
			<body>
				<h2>Documents</h2>
				<table class="doc">

					<tr>
						<th>Number</th>
						<th>Total Items</th>
						<th>Created</th>
						<th>Status</th>
						<th>Type</th>
						<th>Discount</th>
						<th>Net Total</th>
						<th>Has Been Checked</th>
						<th>Has Been Executed</th>
						<th>Finalized</th>
						<th>Total</th>
					</tr>

					<xsl:for-each select="Documents/*">
						<tr>
							<xsl:for-each select="*">
								<td>
									<xsl:value-of select="." />
								</td>
							</xsl:for-each>
						</tr>
					</xsl:for-each>

				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>