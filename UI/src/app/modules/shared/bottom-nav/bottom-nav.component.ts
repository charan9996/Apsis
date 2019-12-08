import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'app-bottom-nav',
    templateUrl: './bottom-nav.component.html',
    styleUrls: ['./bottom-nav.component.css']
})
export class BottomNavComponent implements OnInit, OnChanges {
    @Input() public pageCount: number;
    @Input() public currentPage: number;
    @Output('pageListener') public switchPage = new EventEmitter<number>();
    public pages: number[] = [];
    public nextPage: boolean = true;
    public previousPage: boolean = true;
    constructor() {}
    ngOnInit() {}
    ngOnChanges(changes: SimpleChanges) {
        if (changes['pageCount']) {
            this.pages = [];
            for (var i = 0; i < this.pageCount; i++) {
                this.pages.push(i + 1);
            }
            if (this.currentPage == 1) {
                this.previousPage = false;
            }
            console.log('page count' + this.pageCount + ' ' + this.pages.length);
            if (this.pages.length == 1) {
                this.nextPage = false;
                this.previousPage = false;
            }
            if (this.pages.length > 1) this.nextPage = true;
        }
        if (changes['currentPage']) {
            console.log('changed current page to : ' + this.currentPage);
            if (this.currentPage == 0 || this.currentPage == undefined) this.currentPage = 1;
            if (this.currentPage == 1) {
                this.previousPage = false;
            }
            if (this.currentPage < this.pages.length) {
                this.nextPage = true;
            }
            if (this.currentPage == this.pages.length) {
                this.nextPage = false;
            }
        }
    }

    public changePage(i: number) {
        this.currentPage = i;
        this.switchPage.emit(i);
        console.log('switch page' + this.currentPage);
        if (this.currentPage == this.pages.length) {
            this.nextPage = false;
        }
        if (this.currentPage < this.pages.length) {
            this.nextPage = true;
        }
        if (this.currentPage > 1) {
            this.previousPage = true;
        }
        if (this.currentPage == 1) {
            this.previousPage = false;
        }
    }

    public goPrevPage() {
        if (this.currentPage - 1 > 0) {
            this.currentPage--;
            this.switchPage.emit(this.currentPage);
            console.log('hit prev' + this.currentPage);
        }
        if (this.currentPage == 1) {
            this.previousPage = false;
        }
        if (this.currentPage < this.pages.length) {
            this.nextPage = true;
        }
    }

    public goNextPage() {
        if (this.currentPage + 1 <= this.pages.length) {
            this.currentPage++;
            this.switchPage.emit(this.currentPage);
        }
        if (this.currentPage == this.pages.length) {
            this.nextPage = false;
        }
        if (this.currentPage > 1) {
            this.previousPage = true;
        }
    }
}
