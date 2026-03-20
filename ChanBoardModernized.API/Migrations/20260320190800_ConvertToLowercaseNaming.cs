using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChanBoardModernized.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToLowercaseNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Photos_CommentPhotoId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Threads_ThreadId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Threads",
                table: "Threads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentCounters",
                table: "CommentCounters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Boards",
                table: "Boards");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Threads",
                newName: "threads");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refreshtokens");

            migrationBuilder.RenameTable(
                name: "Photos",
                newName: "photos");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "comments");

            migrationBuilder.RenameTable(
                name: "CommentCounters",
                newName: "commentcounters");

            migrationBuilder.RenameTable(
                name: "Boards",
                newName: "boards");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "passwordhash");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "users",
                newName: "IX_users_username");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "threads",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "threads",
                newName: "createdbyuserid");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "threads",
                newName: "createdby");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "threads",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "CommentCount",
                table: "threads",
                newName: "commentcount");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "threads",
                newName: "boardid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "threads",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Threads_CreatedAt",
                table: "threads",
                newName: "IX_threads_createdat");

            migrationBuilder.RenameIndex(
                name: "IX_Threads_BoardId",
                table: "threads",
                newName: "ix_threads_boardid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refreshtokens",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refreshtokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "ReplacedByToken",
                table: "refreshtokens",
                newName: "replacedbytoken");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "refreshtokens",
                newName: "isrevoked");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refreshtokens",
                newName: "expiresat");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refreshtokens",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refreshtokens",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refreshtokens",
                newName: "IX_refreshtokens_userid");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token",
                table: "refreshtokens",
                newName: "IX_refreshtokens_token");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "photos",
                newName: "width");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "photos",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "photos",
                newName: "uploadedat");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "photos",
                newName: "originalfilename");

            migrationBuilder.RenameColumn(
                name: "Height",
                table: "photos",
                newName: "height");

            migrationBuilder.RenameColumn(
                name: "FileSizeBytes",
                table: "photos",
                newName: "filesizebytes");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "photos",
                newName: "contenttype");

            migrationBuilder.RenameColumn(
                name: "BlobPath",
                table: "photos",
                newName: "blobpath");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "photos",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Photos_UserId",
                table: "photos",
                newName: "IX_photos_userid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "comments",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "comments",
                newName: "threadid");

            migrationBuilder.RenameColumn(
                name: "TextContent",
                table: "comments",
                newName: "textcontent");

            migrationBuilder.RenameColumn(
                name: "PostDigits",
                table: "comments",
                newName: "postdigits");

            migrationBuilder.RenameColumn(
                name: "DisplayAuthor",
                table: "comments",
                newName: "displayauthor");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "comments",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "CommentPhotoId",
                table: "comments",
                newName: "commentphotoid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "comments",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "comments",
                newName: "ix_comments_userid");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ThreadId",
                table: "comments",
                newName: "ix_comments_threadid");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CreatedAt",
                table: "comments",
                newName: "IX_comments_createdat");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CommentPhotoId",
                table: "comments",
                newName: "ix_comments_commentphotoid");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "commentcounters",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "commentcounters",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "commentcounters",
                newName: "boardid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "commentcounters",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_CommentCounters_BoardId",
                table: "commentcounters",
                newName: "IX_commentcounters_boardid");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "boards",
                newName: "shortname");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "boards",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "boards",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "boards",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Boards_ShortName",
                table: "boards",
                newName: "IX_boards_shortname");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_threads",
                table: "threads",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refreshtokens",
                table: "refreshtokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_photos",
                table: "photos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comments",
                table: "comments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_commentcounters",
                table: "commentcounters",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_boards",
                table: "boards",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_photos_commentphotoid",
                table: "comments",
                column: "commentphotoid",
                principalTable: "photos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_threads_threadid",
                table: "comments",
                column: "threadid",
                principalTable: "threads",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_userid",
                table: "comments",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_threads_boards_boardid",
                table: "threads",
                column: "boardid",
                principalTable: "boards",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_photos_commentphotoid",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_threads_threadid",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_userid",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_threads_boards_boardid",
                table: "threads");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_threads",
                table: "threads");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refreshtokens",
                table: "refreshtokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_photos",
                table: "photos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comments",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_commentcounters",
                table: "commentcounters");

            migrationBuilder.DropPrimaryKey(
                name: "pk_boards",
                table: "boards");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "threads",
                newName: "Threads");

            migrationBuilder.RenameTable(
                name: "refreshtokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "photos",
                newName: "Photos");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "commentcounters",
                newName: "CommentCounters");

            migrationBuilder.RenameTable(
                name: "boards",
                newName: "Boards");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "passwordhash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_users_username",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Threads",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "createdbyuserid",
                table: "Threads",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "createdby",
                table: "Threads",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "Threads",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "commentcount",
                table: "Threads",
                newName: "CommentCount");

            migrationBuilder.RenameColumn(
                name: "boardid",
                table: "Threads",
                newName: "BoardId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Threads",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_threads_createdat",
                table: "Threads",
                newName: "IX_Threads_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_threads_boardid",
                table: "Threads",
                newName: "IX_Threads_BoardId");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "replacedbytoken",
                table: "RefreshTokens",
                newName: "ReplacedByToken");

            migrationBuilder.RenameColumn(
                name: "isrevoked",
                table: "RefreshTokens",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "expiresat",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RefreshTokens",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_refreshtokens_userid",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refreshtokens_token",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token");

            migrationBuilder.RenameColumn(
                name: "width",
                table: "Photos",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Photos",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "uploadedat",
                table: "Photos",
                newName: "UploadedAt");

            migrationBuilder.RenameColumn(
                name: "originalfilename",
                table: "Photos",
                newName: "OriginalFileName");

            migrationBuilder.RenameColumn(
                name: "height",
                table: "Photos",
                newName: "Height");

            migrationBuilder.RenameColumn(
                name: "filesizebytes",
                table: "Photos",
                newName: "FileSizeBytes");

            migrationBuilder.RenameColumn(
                name: "contenttype",
                table: "Photos",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "blobpath",
                table: "Photos",
                newName: "BlobPath");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Photos",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_photos_userid",
                table: "Photos",
                newName: "IX_Photos_UserId");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "threadid",
                table: "Comments",
                newName: "ThreadId");

            migrationBuilder.RenameColumn(
                name: "textcontent",
                table: "Comments",
                newName: "TextContent");

            migrationBuilder.RenameColumn(
                name: "postdigits",
                table: "Comments",
                newName: "PostDigits");

            migrationBuilder.RenameColumn(
                name: "displayauthor",
                table: "Comments",
                newName: "DisplayAuthor");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "Comments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "commentphotoid",
                table: "Comments",
                newName: "CommentPhotoId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Comments",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_userid",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_comments_threadid",
                table: "Comments",
                newName: "IX_Comments_ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_createdat",
                table: "Comments",
                newName: "IX_Comments_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_comments_commentphotoid",
                table: "Comments",
                newName: "IX_Comments_CommentPhotoId");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "CommentCounters",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "CommentCounters",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "boardid",
                table: "CommentCounters",
                newName: "BoardId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CommentCounters",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_commentcounters_boardid",
                table: "CommentCounters",
                newName: "IX_CommentCounters_BoardId");

            migrationBuilder.RenameColumn(
                name: "shortname",
                table: "Boards",
                newName: "ShortName");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Boards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Boards",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Boards",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_boards_shortname",
                table: "Boards",
                newName: "IX_Boards_ShortName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Threads",
                table: "Threads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentCounters",
                table: "CommentCounters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Boards",
                table: "Boards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Photos_CommentPhotoId",
                table: "Comments",
                column: "CommentPhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Threads_ThreadId",
                table: "Comments",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Boards_BoardId",
                table: "Threads",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
