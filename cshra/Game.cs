using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms.VisualStyles;

namespace CSHra
{

    enum PressedArrow { none, left, up, right, down };

    public enum State { notStarted, runs, runsBack, loss, ready };

    abstract class MovingObject
    {
        public Map map;
        public int x; 
        public int y;
        public int dirX;
        public int dirY;
        public double speed;
        public double offsetX = 0;
        public double offsetY = 0;
        public int defaultX;
        public int defaultY;

        public abstract void Move();

        public bool CanMove(int drx, int dry)
        {
            return !map.isWall(x + drx, y + dry);
        }
    }

    class Hero : MovingObject
    {
        public int desiredDirx = 1;
        public int desiredDiry = 0;
        public double normalSpeed = 0.1;
        public double altSpeed = 0.125;

        public Hero(Map map, int xx, int yy)
        {
            this.map = map;
            this.x = xx;
            this.y = yy;
            this.dirX = 1;
            this.dirY = 0;
            this.speed = normalSpeed;
            this.defaultX = xx;
            this.defaultY = yy;

        }
        public override void Move()
        {
             // Desired direction change
            if (CanMove(desiredDirx, desiredDiry))
            {
                this.dirX = desiredDirx;
                this.dirY = desiredDiry;
            }

            // Calculate potential new positions with current offset
            double newOffsetX = this.offsetX + this.dirX * this.speed;
            double newOffsetY = this.offsetY + this.dirY * this.speed;

            // Check if the next position has a wall
            if (map.isWall(x + this.dirX, y + (newOffsetY > 0 ? 1 : (newOffsetY < 0 ? -1 : 0)))) return;
            if (map.isWall(x + (newOffsetX > 0 ? 1 : (newOffsetX < 0 ? -1 : 0)), y + this.dirY)) return;

            // Update offsets
            this.offsetX += this.dirX * this.speed;
            this.offsetY += this.dirY * this.speed;

            int new_x = x;
            int new_y = y;

            // Move to a new tile
            if (Math.Abs(this.offsetX) >= 1)
            {
                new_x += Math.Sign(this.offsetX);
                this.offsetX = 0;
            }
            if (Math.Abs(this.offsetY) >= 1)
            {
                new_y += Math.Sign(this.offsetY);
                this.offsetY = 0;
            }

            // Move to the new position if it has changed
            if (new_x != x || new_y != y)
            {
                map.changePosition(x, y, new_x, new_y);
            }
        }
    
       public void keys()
        {
            switch (map.pressedArrow)
            {
                case PressedArrow.none:
                    break;
                case PressedArrow.left:
                    this.desiredDirx = -1;
                    this.desiredDiry = 0;
                    break;
                case PressedArrow.up:
                    this.desiredDirx = 0;
                    this.desiredDiry = -1;
                    break;
                case PressedArrow.right:
                    this.desiredDirx = 1;
                    this.desiredDiry = 0;
                    break;
                case PressedArrow.down:
                    this.desiredDirx = 0;
                    this.desiredDiry = 1;
                    break;
                default:
                    break;
            }
        }
    }

    class Ghost : MovingObject
    {
        private Random random = new Random();
        public bool normal = true;

        public double normalSpeed = 0.1;
        public double altSpeed = 0.0625;

        public Ghost(Map map, int xx, int yy){
            this.map = map;
            this.x = xx;
            this.y = yy;
            this.dirX = 0;
            this.dirY = -1;
            this.speed = normalSpeed;
            this.normal = true;
            this.defaultX = xx;
            this.defaultY = yy;
        }

        public override void Move()
        {
            // Calculate potential new positions with current offset
            double newOffsetX = this.offsetX + this.dirX * this.speed;
            double newOffsetY = this.offsetY + this.dirY * this.speed;

            // Check if the next position has a wall
            if (map.isWall(x + this.dirX, y + (newOffsetY > 0 ? 1 : (newOffsetY < 0 ? -1 : 0))) || 
                map.isWall(x + (newOffsetX > 0 ? 1 : (newOffsetX < 0 ? -1 : 0)), y + this.dirY)) 
            {
                this.decideDirecton(!this.normal);
                return;
            }

            // Update offsets
            this.offsetX += this.dirX * this.speed;
            this.offsetY += this.dirY * this.speed;

            int new_x = x;
            int new_y = y;

            // Move to a new tile
            if (Math.Abs(this.offsetX) >= 1)
            {
                new_x += Math.Sign(this.offsetX);
                this.offsetX = 0;
            }
            if (Math.Abs(this.offsetY) >= 1)
            {
                new_y += Math.Sign(this.offsetY);
                this.offsetY = 0;
            }

            // Move to the new position if it has changed
            if (new_x != x || new_y != y)
            {
                this.x = new_x;
                this.y = new_y;
                
                // Teleporting
                if (this.x == 0 && this.y == map.height / 2 - 1 && this.dirX == -1){
                    this.x = map.width - 1;
                }
                else if (this.x == map.width - 1 && this.y == map.height / 2 - 1 && this.dirX == 1){
                    this.x = 0;
                }
                else{
                    this.decideDirecton(!this.normal);
                }
            }


        }

        public void decideDirecton(bool reverse){

            
            int[,] vectors = { {1, 0}, {-1, 0}, {0, 1}, {0, -1} };

            if (random.Next(0, 10) >= 4 + (reverse ? 3 : 0)){
                
                // random direction
                int vector = random.Next(vectors.GetLength(0));

                int newDirX = vectors[vector, 0];
                int newDirY = vectors[vector, 1];

                // Not randomly turning backwards
                if (CanMove(newDirX, newDirY) && !(newDirX == -this.dirX && newDirY == -this.dirY))
                {
                    this.dirX = newDirX;
                    this.dirY = newDirY;      

                }

                return;

            }

            BFS(vectors, reverse);
            
        }

        void BFS(int[,] vectors, bool reverse){

            List<(int, int)> found = new List<(int, int)>();
            Queue<(int a, int b, int c, int d)> queue = new Queue<(int, int, int, int)>();

            // First neighbours
            for (int i = 0; i < vectors.GetLength(0); i++){
                if (!this.map.isWall(this.x + vectors[i, 0], this.y + vectors[i, 1])){
                    queue.Enqueue((this.x + vectors[i, 0], this.y + vectors[i, 1], vectors[i, 0], vectors[i, 1]));
                }
            }

            while (queue.Count > 0)
            {
                (int theX, int theY, int theDirX, int theDirY) = queue.Dequeue();

                // Finding the hero
                if (map.hero.x == theX && map.hero.y == theY)
                {
                    this.dirX = theDirX * (reverse ? -1 : 1);
                    this.dirY = theDirY * (reverse ? -1 : 1);
                    return;
                }

                // Neighbours
                for(int i = 0; i < vectors.GetLength(0); i++)
                {
                    int nx = theX + vectors[i, 0];
                    int ny = theY + vectors[i, 1];
                    
                    if ( !this.map.isWall(nx, ny) && !haveIFound(nx, ny, found) && 0 <= nx && nx <= map.width - 1)
                    {
                        queue.Enqueue((nx, ny, theDirX, theDirY));
                        found.Add((nx, ny));
                    }

                    // Teleporting
                    if (nx == 0 && ny == map.height / 2 - 1 && !haveIFound(map.width -1, map.height / 2 - 1, found))
                    {
                        queue.Enqueue((map.width - 1, map.height / 2 - 1, theDirX, theDirY));
                        found.Add((map.width - 1, map.height / 2 - 1));
                    }
                    else if (nx == map.width - 1 && ny == map.height / 2 - 1 && !haveIFound(0, map.height / 2 - 1, found))
                    {
                        queue.Enqueue((0, map.height / 2 - 1, theDirX, theDirY));
                        found.Add((0, map.height / 2 - 1));
                    }

                }
            }
        }

        private bool haveIFound(int nx, int ny, List<(int, int)> found){
            for (int i = 0; i < found.Count; i++){
                if (found[i].Item1 == nx && found[i].Item2 == ny){
                    return true;
                }
            }
            return false;
        }
    }

    class Map
    {
        char[,] plan;
        public int width;
        public int height;

        public State state = State.notStarted;
        public PressedArrow pressedArrow;

        Bitmap[] icons;
        int tileSize;
        int timer;

        public Hero hero;
        public List<Ghost> ghosts;

        int score = 0;
        int lives = 3;
        int pointsToCollect = 0;

        const int startingTimer = 1300;
        const int modeTimer = 6000;
    
        public Map(string mapDir, string iconsDir)
        {
            loadIcons(iconsDir);
            loadMap(mapDir);
            state = State.ready;
            timer = startingTimer;

            for(int i = 0; i < ghosts.Count; i++){
                ghosts[i].decideDirecton(false);
            }
        }

        public void loadMap(string cesta)
        {

            System.IO.StreamReader sr = new System.IO.StreamReader("../../../Resources/" + cesta);
            width = int.Parse(sr.ReadLine());
            height = int.Parse(sr.ReadLine());
            plan = new char[width, height];

            ghosts = new List<Ghost>();

            for (int y = 0; y < height; y++)
            {
                string line = sr.ReadLine();
                for (int x = 0; x < width; x++)
                {
                    char chr = line[x];
                    plan[x, y] = chr;

                    if (chr == 'H') this.hero = new Hero(this, x, y);
                    if (chr == 'I' || chr == 'O') pointsToCollect += 1;     // Counting number of points
                    if (chr == 'G'){
                        ghosts.Add(new Ghost(this, x, y));
                        plan[x, y] = 'E';       // Ghosts arent on the map
                    }
                }
            }
            sr.Close();
           
        }

        public void loadIcons(string cesta)
        {
            Bitmap bmp = new Bitmap("../../../Resources/" + cesta);
            this.tileSize = bmp.Height;     // 30
            int n = bmp.Width / tileSize;
            icons = new Bitmap[n];
            for (int i = 0; i < n; i++)
            {
                Rectangle rect = new Rectangle(i * tileSize, 0, tileSize, tileSize);
                icons[i] = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
            }
        }

        public bool isWall(int x, int y){
            try{
                return plan[x, y] == 'X';
            }
            catch(IndexOutOfRangeException){
                return false;
            }
        }

        public void changePosition(int zX, int zY, int naX, int naY)
        {
            // Teleporting
            if (zX == 0 && zY == height / 2 - 1 && hero.dirX == -1){
                naX = width - 1;
            }
            else if (zX == width - 1 && zY == height / 2 - 1 && hero.dirX == 1){
                naX = 0;
            }

            char c = plan[zX, zY];
            plan[zX, zY] = 'E';     // Clearing the old position

            if (plan[naX, naY] == 'I') {
                score += 10;
                pointsToCollect--;
            }
            else if (plan[naX, naY] == 'O') {
                score += 50;
                pointsToCollect--;
                startReverseMode();
            }

            plan[naX, naY] = c;

            // Move the hero
            if (c == 'H')
            {
                hero.x = naX;
                hero.y = naY;
            }

            // New game
            if (pointsToCollect == 0)
            {
                loadMap("map.txt");
                state = State.ready;
                timer = startingTimer;

            }
        }

        void startReverseMode(){
            state = State.runsBack;
            timer = modeTimer;
            hero.speed = hero.altSpeed;
            for (int i = 0; i < ghosts.Count; i++){
                ghosts[i].normal = false;
                ghosts[i].speed = ghosts[i].altSpeed;
            }
        }

        public void timerChange(){
            this.timer -= 16;
            if (this.timer <= 0){
                if (state == State.ready){
                    // Starts game
                    state = State.runs;
                }
                else if (state == State.runsBack){
                    // Normal mode
                    state = State.runs;
                    for (int i = 0; i < ghosts.Count; i++){
                        ghosts[i].normal = true;
                        ghosts[i].speed = ghosts[i].normalSpeed;
                    }
                    hero.speed = hero.normalSpeed;
                }
            }
        }

        public void Draw(Graphics g)
        {

            // Draw background and points
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char c = plan[x, y];
                    if (c == 'H' || c == 'G') continue;

                    int imageIndex = "EXIO".IndexOf(c);
                    
                    g.DrawImage(icons[imageIndex], x * tileSize, y * tileSize);
                }
            }

            // Draw hero
            int playerImageIndex = 4 + 2 * (1 - this.hero.dirX + (this.hero.dirY == -1 ? 2 : 0)) + ((int)((this.hero.offsetX + this.hero.offsetY) / 0.25) % 2 == 0 ? 0 : 1);
 
            g.DrawImage(icons[0], this.hero.x * tileSize, this.hero.y * tileSize);
            g.DrawImage(icons[playerImageIndex], (int)((this.hero.x + this.hero.offsetX) * tileSize), (int)((this.hero.y + this.hero.offsetY) * tileSize));

            // Draw ghosts
            for (int i = 0; i < ghosts.Count; i++)
            {
                int ghostImageIndex = 12 + 4 * i + 1 - ghosts[i].dirX + (ghosts[i].dirY == -1 ? 2 : 0);
                if (!ghosts[i].normal) {
                    if (timer < 2000 && (timer / 250) % 2 == 0 ){       // Flashing
                        ghostImageIndex = 12 + 4 * ghosts.Count + 1;
                    }
                    else {
                        ghostImageIndex = 12 + 4 * ghosts.Count;
                    }
                }
                
                g.DrawImage(icons[ghostImageIndex], (int)((ghosts[i].x + ghosts[i].offsetX) * tileSize), (int)((ghosts[i].y + ghosts[i].offsetY) * tileSize));
            }

            // Draw score
            g.DrawString("" + score, new Font("Courier New", 18, FontStyle.Bold), Brushes.White, 5, 3);

            // Draw lives
            for (int i = 0; i < lives; i++){
                g.DrawImage(icons[4], (width - 1 - i) * tileSize, 0);
            }

            // Draw ready text
            if (state == State.ready){
                g.DrawString("READY!", new Font("Courier New", 18, FontStyle.Bold), Brushes.Yellow, (width / 2 - 1) * tileSize, (height / 2 + 1) * tileSize + 3);
            }

            // Draw game over text
            else if (state == State.loss){
                for (int i = 0; i < 5; i++){
                    g.DrawImage(icons[0], (width / 2 - 2 + i) * tileSize, (height / 2 + 1 ) * tileSize);
                }
                
                g.DrawString("GAME OVER!", new Font("Courier New", 18, FontStyle.Bold), Brushes.Red, (width / 2 - 2) * tileSize, (height / 2 + 1) * tileSize + 3);
            }

        }

        public void MoveEverything(PressedArrow pressedArrow)
        {
            this.pressedArrow = pressedArrow;

            hero.keys();
            hero.Move();

            for (int i = 0; i < ghosts.Count; i++)
            {
                ghosts[i].Move();
                if (checkCollision(ghosts[i])) break;
                
            }
        }

        bool checkCollision(Ghost ghost){
            if (ghost.x == hero.x && ghost.y == hero.y){
                if (ghost.normal){
                    lives--;
                    if (lives <= 0){
                        // Game over
                        state = State.loss;
                    }
                    else{
                        // Restart game after death
                        state = State.ready;
                        timer = startingTimer;
                        for (int j = 0; j < ghosts.Count; j++){
                            ghosts[j] = new Ghost(this, ghosts[j].defaultX, ghosts[j].defaultY);
                            ghosts[j].decideDirecton(false);
                        }
                        
                        changePosition(hero.x, hero.y, hero.defaultX, hero.defaultY);
                        
                        this.hero = new Hero(this, hero.x, hero.y);

                        return true;
                    }
                }
                else{
                    // Eating a ghost
                    int eaten = 0;
                    for (int j = 0; j < ghosts.Count; j++){
                        if (ghosts[j].normal){
                            eaten++;
                        }
                    }
                    if (eaten == ghosts.Count - 1) {
                        state = State.runs;
                        hero.speed = hero.normalSpeed;
                    }

                    score += 200 * (int)(Math.Pow(2, eaten));

                    // Restarting ghost
                    ghost.normal = true;
                    ghost.speed = ghost.normalSpeed;
                    ghost.x = ghost.defaultX;
                    ghost.y = ghost.defaultY;
                    ghost.offsetX = 0;
                    ghost.offsetY = 0;
                                            
                }
                
            }

            return false;
        }
    }
}
