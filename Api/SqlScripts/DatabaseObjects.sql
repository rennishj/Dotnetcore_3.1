use Movies
go

if not exists (select  1 from information_schema.tables t where t.TABLE_SCHEMA='dbo' and t.TABLE_NAME='Movie')
begin
    create table dbo.Movie
(
   Id int not null identity(1,1)  constraint pk_movie_id primary key clustered,
   Title varchar(500) not null,
   Description varchar(500) not null,
   Genre varchar(10) not null,
   ReleaseDate datetime,
   Director varchar(100) not null,
   CreatedOn datetime not null constraint df_movie_createdon default getdate()
)
end
else
  print ('table dbo.Movie already exists')
go


if not exists (select  1 from information_schema.tables t where t.TABLE_SCHEMA='dbo' and t.TABLE_NAME='Poster')
begin
    create table dbo.Poster
(
   Id int not null identity(1,1)  constraint pk_poster_id primary key clustered,
   MovieId int not null constraint  fk_poster_movie_id  foreign key references dbo.Movie(Id),
   Name varchar(200) not null,
   Bytes varbinary(max) not null,
   CreatedOn datetime not null constraint df_poster_createdon default getdate()
)
end
else
  print ('table dbo.Poster already exists')
go

if object_id('dbo.CreateMovie', 'p') is not null
begin
	drop procedure dbo.CreateMovie
end
go
create procedure dbo.CreateMovie
(
   @Id int output,
   @Title varchar(100),
   @Description varchar(100),
   @Genre varchar(10),
   @ReleaseDate datetime,
   @Director varchar(100)
)
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	insert into dbo.Movie
	(		 
	  Title,
	  Description,
	  Genre,
	  ReleaseDate,
      Director
	)
	values
	(
	   @Title,
	   @Description,
	   @Genre,
	   @ReleaseDate,
	   @Director
	)

	select @Id = SCOPE_IDENTITY()
end
go

if object_id('dbo.GetAllMovies', 'p') is not null
begin
	drop procedure dbo.GetAllMovies
end
go
create procedure dbo.GetAllMovies
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	select
		Title,
		Description,
		Genre,
		ReleaseDate,
		Director
	from
		dbo.Movie
end
go

if object_id('dbo.GetMovieById', 'p') is not null
begin
	drop procedure dbo.GetMovieById
end
go
create procedure dbo.GetMovieById
(
  @MovieId int
)
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	select
	    m.Id,
		m.Title,
		m.Description,
		m.Genre,
		m.ReleaseDate,
		m.Director,
		m.CreatedOn
	from
		dbo.Movie m
	where
		m.Id  =@MovieId
end

if object_id('dbo.UpdateMovieById', 'p') is not null
begin
	drop procedure dbo.UpdateMovieById
end
go
create procedure dbo.UpdateMovieById
(
  @MovieId int,
  @Title varchar(500),
  @Description varchar(500),
  @Genre varchar(10),
  @ReleaseDate datetime,
  @Director varchar(100)
)
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	Update
		dbo.Movie
	set
		Title = @Title,
		Description = @Description,
		Genre = @Genre,
		Director = @Director,
		ReleaseDate = @ReleaseDate
	where
		Id = @MovieId
end
go

if object_id('dbo.DeleteMovieById', 'p') is not null
begin
	drop procedure dbo.DeleteMovieById
end
go
create procedure dbo.DeleteMovieById
(
  @MovieId int
)
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	delete	
		dbo.Movie
	where
		 Id= @MovieId
end

--poster

if object_id('dbo.CreatePoster', 'p') is not null
begin
	drop procedure dbo.CreatePoster
end
go
create procedure dbo.CreatePoster
(
   @Id int output,
   @MovieId int,
   @Name varchar(200),
   @Bytes varbinary(max)
)
as
begin
    set nocount on;
	set transaction isolation level read uncommitted;
	insert into dbo.Poster
	(		 
	  MovieId,
	  Name,
	  Bytes
	)
	values
	(
	   @MovieId,
	   @Name,
	   @Bytes
	)

	select @Id = SCOPE_IDENTITY()
end
go